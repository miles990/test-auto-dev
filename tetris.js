// 遊戲設定
const COLS = 10;
const ROWS = 20;
const BLOCK_SIZE = 30;
const LINES_PER_LEVEL = 10;

// 顏色定義
const COLORS = {
    'I': '#00f0f0',
    'O': '#f0f000',
    'T': '#a000f0',
    'S': '#00f000',
    'Z': '#f00000',
    'J': '#0000f0',
    'L': '#f0a000',
    'empty': '#0f0f1e',
    'grid': '#1a1a2e'
};

// 方塊形狀定義
const SHAPES = {
    'I': [
        [[0, 0, 0, 0],
         [1, 1, 1, 1],
         [0, 0, 0, 0],
         [0, 0, 0, 0]]
    ],
    'O': [
        [[1, 1],
         [1, 1]]
    ],
    'T': [
        [[0, 1, 0],
         [1, 1, 1],
         [0, 0, 0]],
        [[0, 1, 0],
         [0, 1, 1],
         [0, 1, 0]],
        [[0, 0, 0],
         [1, 1, 1],
         [0, 1, 0]],
        [[0, 1, 0],
         [1, 1, 0],
         [0, 1, 0]]
    ],
    'S': [
        [[0, 1, 1],
         [1, 1, 0],
         [0, 0, 0]],
        [[0, 1, 0],
         [0, 1, 1],
         [0, 0, 1]]
    ],
    'Z': [
        [[1, 1, 0],
         [0, 1, 1],
         [0, 0, 0]],
        [[0, 0, 1],
         [0, 1, 1],
         [0, 1, 0]]
    ],
    'J': [
        [[1, 0, 0],
         [1, 1, 1],
         [0, 0, 0]],
        [[0, 1, 1],
         [0, 1, 0],
         [0, 1, 0]],
        [[0, 0, 0],
         [1, 1, 1],
         [0, 0, 1]],
        [[0, 1, 0],
         [0, 1, 0],
         [1, 1, 0]]
    ],
    'L': [
        [[0, 0, 1],
         [1, 1, 1],
         [0, 0, 0]],
        [[0, 1, 0],
         [0, 1, 0],
         [0, 1, 1]],
        [[0, 0, 0],
         [1, 1, 1],
         [1, 0, 0]],
        [[1, 1, 0],
         [0, 1, 0],
         [0, 1, 0]]
    ]
};

// 遊戲狀態
class Game {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.nextCanvas = document.getElementById('nextPieceCanvas');
        this.nextCtx = this.nextCanvas.getContext('2d');

        this.board = this.createBoard();
        this.score = 0;
        this.level = 1;
        this.lines = 0;
        this.gameOver = false;
        this.isPaused = false;
        this.isStarted = false;

        this.currentPiece = null;
        this.nextPiece = null;
        this.dropCounter = 0;
        this.dropInterval = 1000;
        this.lastTime = 0;

        this.setupEventListeners();
    }

    createBoard() {
        return Array(ROWS).fill(null).map(() => Array(COLS).fill(0));
    }

    setupEventListeners() {
        document.addEventListener('keydown', (e) => this.handleKeyPress(e));
        document.getElementById('startButton').addEventListener('click', () => this.start());
        document.getElementById('restartButton').addEventListener('click', () => this.restart());
    }

    start() {
        if (!this.isStarted) {
            this.isStarted = true;
            this.currentPiece = this.createPiece();
            this.nextPiece = this.createPiece();
            document.getElementById('startButton').textContent = '重新開始';
            this.hideOverlay();
            this.gameLoop();
        } else {
            this.restart();
        }
    }

    restart() {
        this.board = this.createBoard();
        this.score = 0;
        this.level = 1;
        this.lines = 0;
        this.gameOver = false;
        this.isPaused = false;
        this.dropInterval = 1000;
        this.currentPiece = this.createPiece();
        this.nextPiece = this.createPiece();
        this.updateScore();
        this.hideOverlay();
        this.gameLoop();
    }

    createPiece() {
        const types = Object.keys(SHAPES);
        const type = types[Math.floor(Math.random() * types.length)];
        return {
            type: type,
            shape: SHAPES[type][0],
            rotation: 0,
            x: Math.floor(COLS / 2) - Math.floor(SHAPES[type][0][0].length / 2),
            y: 0
        };
    }

    handleKeyPress(e) {
        if (!this.isStarted || this.gameOver) return;

        if (e.key === ' ') {
            e.preventDefault();
            this.togglePause();
            return;
        }

        if (this.isPaused) return;

        switch(e.key) {
            case 'ArrowLeft':
                e.preventDefault();
                this.move(-1);
                break;
            case 'ArrowRight':
                e.preventDefault();
                this.move(1);
                break;
            case 'ArrowDown':
                e.preventDefault();
                this.drop();
                break;
            case 'ArrowUp':
                e.preventDefault();
                this.rotate();
                break;
        }
    }

    move(dir) {
        this.currentPiece.x += dir;
        if (this.collision()) {
            this.currentPiece.x -= dir;
        }
    }

    rotate() {
        const shapes = SHAPES[this.currentPiece.type];
        const nextRotation = (this.currentPiece.rotation + 1) % shapes.length;
        const previousShape = this.currentPiece.shape;

        this.currentPiece.shape = shapes[nextRotation];
        this.currentPiece.rotation = nextRotation;

        if (this.collision()) {
            this.currentPiece.shape = previousShape;
            this.currentPiece.rotation = (nextRotation - 1 + shapes.length) % shapes.length;
        }
    }

    drop() {
        this.currentPiece.y++;
        if (this.collision()) {
            this.currentPiece.y--;
            this.merge();
            this.clearLines();
            this.currentPiece = this.nextPiece;
            this.nextPiece = this.createPiece();

            if (this.collision()) {
                this.endGame();
            }
        }
        this.dropCounter = 0;
    }

    collision() {
        const shape = this.currentPiece.shape;
        for (let y = 0; y < shape.length; y++) {
            for (let x = 0; x < shape[y].length; x++) {
                if (shape[y][x]) {
                    const newX = this.currentPiece.x + x;
                    const newY = this.currentPiece.y + y;

                    if (newX < 0 || newX >= COLS || newY >= ROWS) {
                        return true;
                    }

                    if (newY >= 0 && this.board[newY][newX]) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    merge() {
        const shape = this.currentPiece.shape;
        for (let y = 0; y < shape.length; y++) {
            for (let x = 0; x < shape[y].length; x++) {
                if (shape[y][x]) {
                    const newY = this.currentPiece.y + y;
                    const newX = this.currentPiece.x + x;
                    if (newY >= 0) {
                        this.board[newY][newX] = this.currentPiece.type;
                    }
                }
            }
        }
    }

    clearLines() {
        let linesCleared = 0;

        for (let y = ROWS - 1; y >= 0; y--) {
            if (this.board[y].every(cell => cell !== 0)) {
                this.board.splice(y, 1);
                this.board.unshift(Array(COLS).fill(0));
                linesCleared++;
                y++;
            }
        }

        if (linesCleared > 0) {
            this.lines += linesCleared;
            this.score += this.calculateScore(linesCleared);
            this.level = Math.floor(this.lines / LINES_PER_LEVEL) + 1;
            this.dropInterval = Math.max(100, 1000 - (this.level - 1) * 100);
            this.updateScore();
        }
    }

    calculateScore(lines) {
        const baseScores = [0, 100, 300, 500, 800];
        return baseScores[lines] * this.level;
    }

    updateScore() {
        document.getElementById('score').textContent = this.score;
        document.getElementById('level').textContent = this.level;
        document.getElementById('lines').textContent = this.lines;
    }

    togglePause() {
        this.isPaused = !this.isPaused;
        if (this.isPaused) {
            this.showOverlay('遊戲暫停', '按 空白鍵 繼續');
        } else {
            this.hideOverlay();
            this.gameLoop();
        }
    }

    endGame() {
        this.gameOver = true;
        this.showOverlay('遊戲結束', `最終分數: ${this.score}`);
    }

    showOverlay(title, message) {
        document.getElementById('overlayTitle').textContent = title;
        document.getElementById('overlayMessage').textContent = message;
        document.getElementById('gameOverlay').classList.remove('hidden');
    }

    hideOverlay() {
        document.getElementById('gameOverlay').classList.add('hidden');
    }

    draw() {
        // 清空畫布
        this.ctx.fillStyle = COLORS.empty;
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // 繪製網格
        this.drawGrid();

        // 繪製已固定的方塊
        this.drawBoard();

        // 繪製當前方塊
        if (this.currentPiece) {
            this.drawPiece(this.currentPiece, this.ctx);
        }

        // 繪製下一個方塊
        if (this.nextPiece) {
            this.drawNextPiece();
        }
    }

    drawGrid() {
        this.ctx.strokeStyle = COLORS.grid;
        this.ctx.lineWidth = 1;

        for (let y = 0; y <= ROWS; y++) {
            this.ctx.beginPath();
            this.ctx.moveTo(0, y * BLOCK_SIZE);
            this.ctx.lineTo(COLS * BLOCK_SIZE, y * BLOCK_SIZE);
            this.ctx.stroke();
        }

        for (let x = 0; x <= COLS; x++) {
            this.ctx.beginPath();
            this.ctx.moveTo(x * BLOCK_SIZE, 0);
            this.ctx.lineTo(x * BLOCK_SIZE, ROWS * BLOCK_SIZE);
            this.ctx.stroke();
        }
    }

    drawBoard() {
        for (let y = 0; y < ROWS; y++) {
            for (let x = 0; x < COLS; x++) {
                if (this.board[y][x]) {
                    this.drawBlock(x, y, COLORS[this.board[y][x]], this.ctx);
                }
            }
        }
    }

    drawPiece(piece, context) {
        const shape = piece.shape;
        for (let y = 0; y < shape.length; y++) {
            for (let x = 0; x < shape[y].length; x++) {
                if (shape[y][x]) {
                    this.drawBlock(piece.x + x, piece.y + y, COLORS[piece.type], context);
                }
            }
        }
    }

    drawBlock(x, y, color, context) {
        context.fillStyle = color;
        context.fillRect(x * BLOCK_SIZE + 1, y * BLOCK_SIZE + 1, BLOCK_SIZE - 2, BLOCK_SIZE - 2);

        // 添加光澤效果
        context.fillStyle = 'rgba(255, 255, 255, 0.2)';
        context.fillRect(x * BLOCK_SIZE + 1, y * BLOCK_SIZE + 1, BLOCK_SIZE - 2, BLOCK_SIZE / 3);
    }

    drawNextPiece() {
        // 清空下一個方塊畫布
        this.nextCtx.fillStyle = 'rgba(255, 255, 255, 0.1)';
        this.nextCtx.fillRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);

        const shape = this.nextPiece.shape;
        const offsetX = (4 - shape[0].length) / 2;
        const offsetY = (4 - shape.length) / 2;

        for (let y = 0; y < shape.length; y++) {
            for (let x = 0; x < shape[y].length; x++) {
                if (shape[y][x]) {
                    this.drawBlock(offsetX + x, offsetY + y, COLORS[this.nextPiece.type], this.nextCtx);
                }
            }
        }
    }

    update(deltaTime) {
        if (this.isPaused || this.gameOver) return;

        this.dropCounter += deltaTime;
        if (this.dropCounter > this.dropInterval) {
            this.drop();
        }
    }

    gameLoop(time = 0) {
        if (this.isPaused || this.gameOver) return;

        const deltaTime = time - this.lastTime;
        this.lastTime = time;

        this.update(deltaTime);
        this.draw();

        requestAnimationFrame((t) => this.gameLoop(t));
    }
}

// 初始化遊戲
let game;
window.addEventListener('load', () => {
    game = new Game();
});
