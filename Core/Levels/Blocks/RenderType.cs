namespace Sharpitecture.Levels.Blocks
{
    public enum RenderType
    {
        Opaque = 0, //Solid block
        Transparent = 1, //Fully or partial transparent blocks
        NoFaceCulling = 2, //Don't cull (omit) neighbouring faces (like leaves)
        Gas = 3, //Basically air
    }
}
