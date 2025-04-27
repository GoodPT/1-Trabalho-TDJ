using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong;

public class Player : Entity
{
    public float Speed;  // Velocidade de movimento do jogador

    // Construtor para inicializar a posição e a velocidade do jogador
    public Player(Vector2 position, float speed) : base(position)
    {
        Speed = speed;  // Define a velocidade do jogador
    }

    // Atualiza a posição do jogador com base nas teclas pressionadas
    public void Update(GameTime gameTime, KeyboardState kstate)
    {
        // Se a tecla para cima (Seta para cima) estiver pressionada
        if (kstate.IsKeyDown(Keys.Up))
        {
            // Move o jogador para cima, com base na velocidade e no tempo passado entre os quadros
            Position.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Se a tecla para baixo (Seta para baixo) estiver pressionada
        if (kstate.IsKeyDown(Keys.Down))
        {
            // Move o jogador para baixo, com base na velocidade e no tempo passado entre os quadros
            Position.Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Garante que o jogador não saia da tela (dentro dos limites definidos)
        // Limita a posição do jogador dentro da altura da tela, levando em consideração a altura da textura
        Position.Y = MathHelper.Clamp(Position.Y,
                                      WorldValues.minBoundaries.Y + Texture.Height / 2,
                                      WorldValues.maxBoundaries.Y - Texture.Height / 2);
    }
}