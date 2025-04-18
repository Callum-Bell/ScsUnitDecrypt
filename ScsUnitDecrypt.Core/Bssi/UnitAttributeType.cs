namespace ScsUnitDecrypt.Core.Bssi
{
    public enum UnitAttributeType
    {
        String = 0x01,
        StringArray = 0x02,
        Token = 0x03,
        TokenArray = 0x04,
        Float = 0x05,
        FloatArray = 0x06,
        Float2 = 0x07,
        Float2Array = 0x08,
        Float3 = 0x09,
        Float3Array = 0x0A,
        Float4 = 0x0B,
        Float4Array = 0x0C,
        Fixed = 0x0D,
        FixedArray = 0x0E,
        Fixed2 = 0x0F,
        Fixed2Array = 0x10,
        Fixed3 = 0x11,
        Fixed3Array = 0x12,
        Fixed4 = 0x13,
        Fixed4Array = 0x14,
        Float4x4 = 0x15,
        Float4x4Array = 0x16,
        Quat = 0x17,
        QuatArray = 0x18,
        Placement = 0x19, // Float7 for V1, Float8 for V2
        PlacementArray = 0x1A, // Float7Array for V1, Float8Array for V2
        PlacementScale = 0x1B,
        PlacementScaleArray = 0x1C,
        PlacementNonUniform = 0x1D,
        PlacementNonUniformArray = 0x1E,
        Plane = 0x1F,
        PlaneArray = 0x20,
        Sphere = 0x21,
        SphereArray = 0x22,
        AaBox = 0x23,
        AaBoxArray = 0x24,
        Int32 = 0x25, // int
        Int32Array = 0x26,
        UInt32 = 0x27, // uint
        UInt32Array = 0x28,
        Int16 = 0x29,
        Int16Array = 0x2A,
        UInt16 = 0x2B,
        UInt16Array = 0x2C,
        Int32_2 = 0x2D, // s32
        Int32Array_2 = 0x2E, // s32 array
        UInt32_2 = 0x2F, // u32
        UInt32Array_2 = 0x30, // u32 array
        Int64 = 0x31,
        Int64Array = 0x32,
        UInt64 = 0x33,
        UInt64Array = 0x34,
        Bool = 0x35,
        BoolArray = 0x36,
        OrdinalString = 0x37,
        Id039 = 0x39,
        IdArray03A = 0x3A,
        Id03B = 0x3B,
        IdArray03C = 0x3C,
        Id03D = 0x3D,
        Int2 = 0x41
    }
}
