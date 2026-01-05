const WebSocket = require('ws');

// 建立 WebSocket 伺服器
const wss = new WebSocket.Server({ port: 8080 });

// 遊戲設定
const GRID_SIZE = 20;
const TILE_COUNT = 30;
const GAME_SPEED = 100;

// 遊戲狀態
let players = {};
let food = [];
let gameLoop = null;

// 顏色列表
const colors = [
    '#FF6B6B', '#4ECDC4', '#45B7D1', '#FFA07A',
    '#98D8C8', '#F7DC6F', '#BB8FCE', '#85C1E2'
];

let colorIndex = 0;

// 生成唯一 ID
function generateId() {
    return Math.random().toString(36).substring(2, 15);
}

// 獲取下一個顏色
function getNextColor() {
    const color = colors[colorIndex % colors.length];
    colorIndex++;
    return color;
}

// 建立新蛇
function createSnake(color) {
    const startX = Math.floor(Math.random() * (TILE_COUNT - 10)) + 5;
    const startY = Math.floor(Math.random() * (TILE_COUNT - 10)) + 5;

    return {
        body: [
            { x: startX, y: startY }
        ],
        direction: { x: 1, y: 0 },
        nextDirection: { x: 1, y: 0 },
        color: color,
        alive: true,
        score: 0
    };
}

// 生成食物
function spawnFood() {
    const newFood = {
        x: Math.floor(Math.random() * TILE_COUNT),
        y: Math.floor(Math.random() * TILE_COUNT)
    };

    // 確保食物不會出現在蛇身上
    let valid = true;
    Object.values(players).forEach(player => {
        if (player.snake) {
            player.snake.body.forEach(segment => {
                if (segment.x === newFood.x && segment.y === newFood.y) {
                    valid = false;
                }
            });
        }
    });

    if (valid) {
        food.push(newFood);
    } else {
        // 如果位置無效，重新生成
        spawnFood();
    }
}

// 廣播給所有玩家
function broadcast(data) {
    const message = JSON.stringify(data);
    Object.values(players).forEach(player => {
        if (player.ws && player.ws.readyState === WebSocket.OPEN) {
            player.ws.send(message);
        }
    });
}

// 更新遊戲狀態
function updateGame() {
    let foodEaten = false;

    Object.entries(players).forEach(([playerId, player]) => {
        if (!player.snake || !player.snake.alive) return;

        const snake = player.snake;

        // 更新方向
        snake.direction = snake.nextDirection;

        // 計算新的頭部位置
        const head = snake.body[0];
        const newHead = {
            x: head.x + snake.direction.x,
            y: head.y + snake.direction.y
        };

        // 檢查撞牆
        if (newHead.x < 0 || newHead.x >= TILE_COUNT ||
            newHead.y < 0 || newHead.y >= TILE_COUNT) {
            snake.alive = false;
            console.log(`玩家 ${playerId} 撞牆死亡`);
            return;
        }

        // 檢查撞到自己
        for (let segment of snake.body) {
            if (segment.x === newHead.x && segment.y === newHead.y) {
                snake.alive = false;
                console.log(`玩家 ${playerId} 撞到自己死亡`);
                return;
            }
        }

        // 檢查撞到其他玩家
        Object.entries(players).forEach(([otherId, otherPlayer]) => {
            if (otherId === playerId || !otherPlayer.snake) return;

            for (let segment of otherPlayer.snake.body) {
                if (segment.x === newHead.x && segment.y === newHead.y) {
                    snake.alive = false;
                    console.log(`玩家 ${playerId} 撞到玩家 ${otherId} 死亡`);
                    return;
                }
            }
        });

        if (!snake.alive) return;

        // 移動蛇
        snake.body.unshift(newHead);

        // 檢查是否吃到食物
        let ate = false;
        food = food.filter(f => {
            if (f.x === newHead.x && f.y === newHead.y) {
                player.score += 10;
                ate = true;
                foodEaten = true;
                console.log(`玩家 ${playerId} 吃到食物，分數: ${player.score}`);
                return false;
            }
            return true;
        });

        if (!ate) {
            snake.body.pop();
        }
    });

    // 如果食物被吃掉或沒有食物，生成新食物
    if (foodEaten || food.length < 3) {
        spawnFood();
    }

    // 廣播遊戲狀態
    const gameState = {
        type: 'update',
        players: Object.entries(players).map(([id, player]) => ({
            id: id,
            snake: player.snake,
            color: player.color,
            score: player.score
        })),
        food: food
    };

    broadcast(gameState);
}

// 開始遊戲循環
function startGameLoop() {
    if (gameLoop) return;

    console.log('開始遊戲循環');
    gameLoop = setInterval(updateGame, GAME_SPEED);
}

// 停止遊戲循環
function stopGameLoop() {
    if (gameLoop) {
        clearInterval(gameLoop);
        gameLoop = null;
        console.log('停止遊戲循環');
    }
}

// 連線處理
wss.on('connection', (ws) => {
    const playerId = generateId();
    const color = getNextColor();

    console.log(`新玩家連線: ${playerId}`);

    // 建立玩家
    players[playerId] = {
        id: playerId,
        ws: ws,
        color: color,
        snake: createSnake(color),
        score: 0
    };

    // 發送初始化資料給新玩家
    ws.send(JSON.stringify({
        type: 'init',
        playerId: playerId,
        color: color
    }));

    // 通知其他玩家
    broadcast({
        type: 'playerJoined',
        playerId: playerId
    });

    // 確保有食物
    if (food.length === 0) {
        for (let i = 0; i < 3; i++) {
            spawnFood();
        }
    }

    // 如果這是第一個玩家，開始遊戲循環
    if (Object.keys(players).length === 1) {
        startGameLoop();
    }

    // 處理訊息
    ws.on('message', (message) => {
        try {
            const data = JSON.parse(message);

            if (data.type === 'move' && players[playerId] && players[playerId].snake) {
                const snake = players[playerId].snake;
                const newDir = data.direction;

                // 檢查是否為有效的轉向（不能直接往反方向）
                if ((snake.direction.x === 0 && newDir.x !== 0) ||
                    (snake.direction.y === 0 && newDir.y !== 0)) {
                    snake.nextDirection = newDir;
                }
            } else if (data.type === 'reset') {
                // 重置玩家的蛇
                if (players[playerId]) {
                    players[playerId].snake = createSnake(players[playerId].color);
                    players[playerId].score = 0;
                    console.log(`玩家 ${playerId} 重新開始`);
                }
            }
        } catch (error) {
            console.error('處理訊息時發生錯誤:', error);
        }
    });

    // 斷線處理
    ws.on('close', () => {
        console.log(`玩家離線: ${playerId}`);

        delete players[playerId];

        // 通知其他玩家
        broadcast({
            type: 'playerLeft',
            playerId: playerId
        });

        // 如果沒有玩家了，停止遊戲循環
        if (Object.keys(players).length === 0) {
            stopGameLoop();
            food = [];
        }
    });

    ws.on('error', (error) => {
        console.error(`玩家 ${playerId} 發生錯誤:`, error);
    });
});

console.log('WebSocket 伺服器運行在 ws://localhost:8080');
console.log('等待玩家連線...');

// 優雅關閉
process.on('SIGINT', () => {
    console.log('\n正在關閉伺服器...');
    stopGameLoop();
    wss.close(() => {
        console.log('伺服器已關閉');
        process.exit(0);
    });
});
