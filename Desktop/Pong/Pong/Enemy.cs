using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong;

// Classe que representa o inimigo no jogo (paddle controlado pela IA)
public class Enemy : Entity
{
    public float Speed;  // Velocidade de movimento do inimigo
    public enum MoveState { UP, DOWN, STOPPED };  // Estados possíveis de movimento do inimigo
    public MoveState State;  // Estado atual do movimento do inimigo

    // Variáveis de IA (previsão do movimento da bola)
    public Vector2? Prediction;  // Ponto previsto de interseção entre a bola e o inimigo
    public double ReactionTime;  // Tempo de reação da IA antes de prever o próximo movimento
    public double LastTimePredicted;  // Última vez que a IA previu a trajetória da bola
    public int Error;  // Margem de erro na previsão (para tornar a IA mais "humana")

    // Construtor para inicializar o inimigo com posição e velocidade
    public Enemy(Vector2 position, float speed) : base(position)
    {
        Speed = speed;
        State = MoveState.STOPPED;  // Inicializa o estado como parado
        ReactionTime = 0.5;  // Tempo de reação padrão
    }

    // Método para atualizar o movimento do inimigo a cada quadro
    public void Update(GameTime gameTime, Ball ball)
    {
        // Se a bola está indo para a direita (não é mais uma ameaça), o inimigo para
        if (ball.Direction.X > 0)
        {
            State = MoveState.STOPPED;
            return;
        }

        // Atualiza o movimento baseado no estado atual
        switch (State)
        {
            case MoveState.UP:
                Position.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;  // Move para cima
                break;
            case MoveState.DOWN:
                Position.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;  // Move para baixo
                break;
            case MoveState.STOPPED:
                break;  // Não faz nada se estiver parado
        }

        // Restringe o movimento do inimigo dentro dos limites do campo
        Position.Y = MathHelper.Clamp(Position.Y, WorldValues.minBoundaries.Y + Texture.Height / 2, WorldValues.maxBoundaries.Y - Texture.Height / 2);

        var currentTime = gameTime.TotalGameTime.TotalSeconds;

        // Somente refaz a previsão se o tempo de reação já tiver passado
        if (Prediction.HasValue && currentTime - LastTimePredicted < ReactionTime)
        {
            return;  // Não faz nada se o tempo de reação não tiver passado
        }

        // Define linhas de projeção para encontrar a interseção entre o movimento da bola e o movimento do inimigo
        var startLine = new Vector2(Position.X, -10000);  // Linha de projeção infinita para cima
        var endLine = new Vector2(Position.X, 10000);  // Linha de projeção infinita para baixo

        var ballStartLine = ball.Position;  // Posição atual da bola
        var ballEndLine = ball.Position + ball.Direction * 10000;  // Projeção do caminho da bola

        // Calcula o ponto de interseção entre as linhas
        var pointOfIntersection = FindIntersection(startLine, endLine, ballStartLine, ballEndLine);

        // Se houver uma previsão de interseção, armazena o ponto
        Prediction = pointOfIntersection.HasValue ? pointOfIntersection.Value : null;

        // Se uma previsão de interseção foi feita, ajusta a IA com base na previsão
        if (Prediction.HasValue)
        {
            LastTimePredicted = (float)gameTime.TotalGameTime.TotalSeconds;  // Atualiza o tempo da última previsão

            // Introduz um erro na previsão para tornar o movimento da IA mais "humano"
            Prediction = new Vector2(Prediction.Value.X, Prediction.Value.Y + new Random().Next(-Error, Error));

            // Atualiza o estado de movimento do inimigo com base na previsão
            if (Prediction.Value.Y >= Position.Y - Texture.Height / 2 && Prediction.Value.Y <= Position.Y + Texture.Height / 2)
            {
                State = MoveState.STOPPED;  // Se o inimigo já está no local certo, ele para
            }
            else if (Prediction.Value.Y < Position.Y)
            {
                State = MoveState.UP;  // Se a previsão está acima, o inimigo vai para cima
            }
            else if (Prediction.Value.Y > Position.Y)
            {
                State = MoveState.DOWN;  // Se a previsão está abaixo, o inimigo vai para baixo
            }
        }
    }

    // Método para calcular a interseção entre duas linhas (usando álgebra linear)
    // Referência: https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
    private static Vector2? FindIntersection(Vector2 aLineStart, Vector2 aLineEnd, Vector2 bLineStart, Vector2 bLineEnd)
    {
        // Calcula o denominador para verificar se as linhas são paralelas
        var denominator = (aLineStart.X - aLineEnd.X) * (bLineStart.Y - bLineEnd.Y) - (aLineStart.Y - aLineEnd.Y) * (bLineStart.X - bLineEnd.X);

        // Se o denominador for zero, as linhas são paralelas e não se interceptam
        if (denominator == 0)
        {
            return null;  // Não há interseção
        }

        // Calcula os coeficientes t e u, que determinam onde ocorre a interseção nas linhas
        var t = ((aLineStart.X - bLineStart.X) * (bLineStart.Y - bLineEnd.Y) - (aLineStart.Y - bLineStart.Y) * (bLineStart.X - bLineEnd.X)) / denominator;
        var u = -((aLineStart.X - aLineEnd.X) * (aLineStart.Y - bLineStart.Y) - (aLineStart.Y - aLineEnd.Y) * (aLineStart.X - bLineStart.X)) / denominator;

        // Verifica se a interseção está dentro dos limites das linhas
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            return new Vector2(aLineStart.X + t * (aLineEnd.X - aLineStart.X), aLineStart.Y + t * (aLineEnd.Y - aLineStart.Y));  // Retorna o ponto de interseção
        }

        return null;  // Não há interseção dentro dos limites das linhas
    }
}
