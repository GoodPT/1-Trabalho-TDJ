using Microsoft.Xna.Framework;

namespace Pong;

// Classe estática que contém os valores de limites (fronteiras) do mundo onde as entidades se movem
public static class WorldValues
{
    // Limite inferior do mundo (minimo), onde as entidades não podem se mover abaixo
    // Este valor é utilizado para restringir o movimento das entidades no eixo Y
    public static Vector2 minBoundaries;

    // Limite superior do mundo (máximo), onde as entidades não podem se mover além
    // Este valor é utilizado para restringir o movimento das entidades no eixo Y
    public static Vector2 maxBoundaries;
}