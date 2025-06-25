using CTFAK.Memory;
using CTFAK.Core.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class FloatParam : ParameterCommon
    {
        public float Value;

        public override void Read(ByteReader reader)
        {
            Value = reader.ReadSingle();
           
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} value: {Value}";
        }
    }
}
