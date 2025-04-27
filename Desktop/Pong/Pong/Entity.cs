using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong;

// Classe base que representa uma entidade genérica no jogo (como o jogador, inimigos, etc.)
// É abstrata para que não possa ser instanciada diretamente, mas serve de base para outras entidades.
public abstract class Entity
{
    // Posição da entidade no mundo (em coordenadas 2D)
    public Vector2 Position;

    // Textura que representa a aparência da entidade no jogo
    public Texture2D Texture;

    // Construtor que inicializa a posição da entidade
    // A textura será atribuída após a criação da entidade
    protected Entity(Vector2 position)
    {
        Position = position;  // Define a posição da entidade
    }

    // Método virtual para desenhar a entidade na tela
    // Este método pode ser sobrescrito em classes derivadas para personalizar o comportamento de desenho
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        // Desenha a textura da entidade no mundo, centralizando a textura na posição
        spriteBatch.Draw(
            Texture,                         // A textura a ser desenhada
            Position,                         // A posição no mundo onde a textura será desenhada
            null,                             // Sem recorte da textura (null significa que não há recorte)
            Color.White,                      // Cor da textura (não altera a cor por padrão)
            0f,                               // Rotação da textura (0 significa sem rotação)
            new Vector2(Texture.Width / 2, Texture.Height / 2), // Ponto de origem para rotação (centro da textura)
            Vector2.One,                      // Escala da textura (1 significa sem escala)
            SpriteEffects.None,               // Efeito de sprite (sem reflexão ou rotação)
            0f);                              // Camada de desenho (quanto maior, mais em cima na tela a textura fica)
    }
}
