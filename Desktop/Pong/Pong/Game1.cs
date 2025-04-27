using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong;

// Enum para determinar o vencedor de cada rodada
public enum Winner
{
    Player,   // O jogador ganhou
    Enemy,    // O inimigo ganhou
    None      // Não houve vencedor (bola fora ou empate)
}

public class Game1 : Game
{
    // Instâncias das entidades do jogo
    public Player player;
    public Enemy enemy;
    public Ball ball;
    public Texture2D board;
    public Texture2D scoreBar;
    public SpriteFont font;
    public int playerPoints, enemyPoints;
    public bool isPaused;

    // Gerenciador de gráficos e SpriteBatch
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        // Inicialização do gerenciador de gráficos
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";  // Diretório para os conteúdos (texturas, sons, etc.)
        IsMouseVisible = true;  // Exibe o cursor do mouse
        isPaused = false;  // Inicialmente o jogo não está pausado
        playerPoints = 0;  // Inicializa a pontuação do jogador
        enemyPoints = 0;   // Inicializa a pontuação do inimigo
    }

    // Método de inicialização do jogo
    protected override void Initialize()
    {
        // Definições de tamanho da tela
        const int scoreBarHeight = 47;   // Altura da barra de placar
        const int boardHeight = 455;     // Altura do tabuleiro
        const int boardWidth = 802;      // Largura do tabuleiro

        // Define o tamanho da janela do jogo
        _graphics.PreferredBackBufferWidth = boardWidth;
        _graphics.PreferredBackBufferHeight = scoreBarHeight + boardHeight;
        _graphics.ApplyChanges();  // Aplica as mudanças de resolução

        // Configuração dos limites do mundo (tamanho do tabuleiro)
        WorldValues.maxBoundaries = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        WorldValues.minBoundaries = new Vector2(0, scoreBarHeight);  // A área útil começa depois da barra de placar

        // Inicialização dos objetos do jogo
        player = new Player(new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight / 2), 500f);
        enemy = new Enemy(new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight / 2), 500f);
        ball = new Ball(new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), 400f);

        base.Initialize();
    }

    // Carrega todos os recursos (texturas, sons, fontes, etc.)
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Carrega as texturas dos objetos
        player.Texture = Content.Load<Texture2D>("assets/arts/Player");
        player.Position.X = _graphics.PreferredBackBufferWidth - player.Texture.Width;

        enemy.Texture = Content.Load<Texture2D>("assets/arts/Computer");
        enemy.Position.X = enemy.Texture.Width;

        ball.Texture = Content.Load<Texture2D>("assets/arts/Ball");

        // Carrega os sons da bola
        string[] songs = { "music/pongblipf4", "music/ponblipg5", "music/pongblipa3", "music/pongblipa4" };
        foreach (string s in songs)
        {
            var song = Content.Load<SoundEffect>(s);
            ball.Songs.Add(song);
        }

        // Carrega as texturas de fundo
        board = Content.Load<Texture2D>("assets/arts/Board");
        scoreBar = Content.Load<Texture2D>("assets/arts/ScoreBar");

        // Carrega a fonte para o placar
        font = Content.Load<SpriteFont>("File");
    }

    // Atualiza o estado do jogo a cada quadro
    protected override void Update(GameTime gameTime)
    {
        // Verifica se o jogador pressionou a tecla de saída (Back ou Escape)
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var kstate = Keyboard.GetState();

        // Se o jogador pressionar "P", o jogo não estará mais pausado
        if (kstate.IsKeyDown(Keys.P))
        {
            isPaused = false;
        }

        // Se o jogador pressionar "R", o jogo é reiniciado
        if (kstate.IsKeyDown(Keys.R))
        {
            playerPoints = 0;
            enemyPoints = 0;
            ResetPositionsAndPause();  // Reseta a posição dos objetos e pausa o jogo
        }

        // Se o jogo não estiver pausado, atualiza as entidades
        if (!isPaused)
        {
            player.Update(gameTime, kstate);  // Atualiza a posição e estado do jogador
            enemy.Update(gameTime, ball);     // Atualiza a posição e estado do inimigo
            ball.Update(gameTime, player, enemy);  // Atualiza a posição e estado da bola

            // Verifica quem é o vencedor e atualiza os pontos
            switch (ball.GetWinner())
            {
                case Winner.Player:
                    playerPoints += 1;  // Incrementa os pontos do jogador
                    ball.Direction = DirectionsHelper.GetRandomDirection(false);  // A bola muda de direção
                    enemy.ReactionTime -= 0.2;  // A reação do inimigo melhora um pouco
                    ResetPositionsAndPause();  // Reseta as posições e pausa o jogo
                    break;
                case Winner.Enemy:
                    enemyPoints += 1;  // Incrementa os pontos do inimigo
                    ball.Direction = DirectionsHelper.GetRandomDirection(true);  // A bola muda de direção
                    enemy.ReactionTime += 0.1;  // A reação do inimigo piora um pouco
                    ResetPositionsAndPause();  // Reseta as posições e pausa o jogo
                    break;
                case Winner.None: break;  // Nenhum vencedor (bola fora ou empate)
            }
        }

        base.Update(gameTime);
    }

    // Desenha o conteúdo na tela
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);  // Limpa a tela com uma cor azul

        _spriteBatch.Begin();  // Inicia a renderização

        // Desenha os componentes do jogo
        DrawBoard(_spriteBatch);
        DrawPlayerScoreBar(_spriteBatch);
        DrawEnemyScoreBar(_spriteBatch);
        player.Draw(_spriteBatch);
        enemy.Draw(_spriteBatch);
        ball.Draw(_spriteBatch);

        // Desenha o placar do jogador e do inimigo
        DrawText(_spriteBatch,
                new Vector2(_graphics.PreferredBackBufferWidth / 2 + 100, scoreBar.Height / 2 - 12),
                playerPoints.ToString());
        DrawText(_spriteBatch,
                new Vector2(_graphics.PreferredBackBufferWidth / 2 - 112, scoreBar.Height / 2 - 12),
                enemyPoints.ToString());

        // Desenha o tempo desde o início do jogo
        DrawText(_spriteBatch,
                new Vector2(_graphics.PreferredBackBufferWidth / 2 - 6, scoreBar.Height / 2 - 12),
                ((int)gameTime.TotalGameTime.TotalSeconds).ToString());

        _spriteBatch.End();  // Finaliza a renderização

        base.Draw(gameTime);
    }

    // Desenha o tabuleiro do jogo
    private void DrawBoard(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(board, new Vector2(0, scoreBar.Height), null, Color.White, 0, Vector2.Zero, new Vector2(1), SpriteEffects.None, 0f);
    }

    // Desenha a barra de placar do jogador
    private void DrawPlayerScoreBar(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(scoreBar, new Vector2(_graphics.PreferredBackBufferWidth, 0), null, Color.White, 0, new Vector2(scoreBar.Width, 0), new Vector2(1), SpriteEffects.FlipHorizontally, 0f);
    }

    // Desenha a barra de placar do inimigo
    private void DrawEnemyScoreBar(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(scoreBar, Vector2.Zero, null, Color.White, 0, Vector2.Zero, new Vector2(1), SpriteEffects.None, 0f);
    }

    // Reseta as posições das entidades e pausa o jogo
    private void ResetPositionsAndPause()
    {
        player.Position = new Vector2(_graphics.PreferredBackBufferWidth - player.Texture.Width, _graphics.PreferredBackBufferHeight / 2);
        enemy.Position = new Vector2(enemy.Texture.Width, _graphics.PreferredBackBufferHeight / 2);
        ball.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        ball.Speed = 400f;  // Reseta a velocidade da bola
        isPaused = true;  // Pausa o jogo
    }

    // Desenha o texto na tela (para o placar ou o tempo)
    private void DrawText(SpriteBatch spriteBatch, Vector2 position, string text)
    {
        var color = Color.White;
        spriteBatch.DrawString(font, text, position, color, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
    }
}
