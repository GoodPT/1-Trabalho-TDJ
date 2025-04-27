using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Pong;

// Classe responsável pelo comportamento da bola no jogo
public class Ball : Entity
{
    // Direção atual do movimento da bola
    public Vector2 Direction;
    // Velocidade da bola
    public float Speed;
    // Lista de efeitos sonoros a tocar nas colisões
    public List<SoundEffect> Songs;

    // Construtor da bola: define posição, velocidade e direção inicial aleatória
    public Ball(Vector2 position, float speed) : base(position)
    {
        Speed = speed;
        Direction = DirectionsHelper.GetRandomDirection(false);
        Songs = new List<SoundEffect>();
    }

    // Atualiza a posição da bola e trata das colisões a cada frame
    public void Update(GameTime gameTime, Player player, Enemy enemy)
    {
        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        HandleWallsCollision();            // Colisões com as paredes
        HandleCollisionWithEntity(enemy);  // Colisão com o inimigo
        HandleCollisionWithEntity(player); // Colisão com o jogador
    }

    // Verifica se a bola saiu dos limites laterais, determinando o vencedor
    public Winner GetWinner()
    {
        if (Position.X - Texture.Width / 2 < WorldValues.minBoundaries.X)
            return Winner.Player;
        else if (Position.X + Texture.Width / 2 > WorldValues.maxBoundaries.X)
            return Winner.Enemy;
        else
            return Winner.None;
    }

    // Toca um som aleatório quando ocorre uma colisão
    private void PlaySound()
    {
        var random = new Random();
        Songs[random.Next(0, Songs.Count)].Play();
    }

    // Trata colisões da bola com as paredes superior e inferior
    private void HandleWallsCollision()
    {
        var ballTopLeft = Position - new Vector2(Texture.Width / 2, Texture.Height / 2);

        if (ballTopLeft.Y < WorldValues.minBoundaries.Y || ballTopLeft.Y + Texture.Height > WorldValues.maxBoundaries.Y)
        {
            Direction.Y *= -1;
            PlaySound();
        }

        if (ballTopLeft.X < WorldValues.minBoundaries.X || ballTopLeft.X + Texture.Width > WorldValues.maxBoundaries.X)
        {
            Direction.X *= -1;
            PlaySound();
        }
    }

    // Mapeia um valor de um intervalo para outro
    private float MapRange(float x, float xMin, float xMax, float yMin, float yMax)
    {
        return yMin + ((yMax - yMin) * (x - xMin)) / (xMax - xMin);
    }

    // Trata colisões da bola com entidades (jogador ou inimigo)
    private void HandleCollisionWithEntity(Entity entity)
    {
        var ballTopLeftPoint = Position - new Vector2(Texture.Width / 2, Texture.Height / 2);
        var ballBottomRight = Position + new Vector2(Texture.Width / 2, Texture.Height / 2);

        var entityTopLeftPoint = entity.Position - new Vector2(entity.Texture.Width / 2, entity.Texture.Height / 2);
        var entityBottomRight = entity.Position + new Vector2(entity.Texture.Width / 2, entity.Texture.Height / 2);

        if (ballTopLeftPoint.X < entityTopLeftPoint.X + entity.Texture.Width &&
            ballTopLeftPoint.X + Texture.Width > entityTopLeftPoint.X &&
            ballTopLeftPoint.Y < entityTopLeftPoint.Y + entity.Texture.Height &&
            ballTopLeftPoint.Y + Texture.Height > entityTopLeftPoint.Y)
        {
            Direction.X *= -1; // Inverte direção horizontal
            Direction.Y = MapRange(Position.Y, entityTopLeftPoint.Y, entityBottomRight.Y, -1f, 1f); // Ajusta direção vertical conforme ponto de impacto
            Speed += 2; // Aumenta a velocidade da bola
            PlaySound();
        }
    }
}
