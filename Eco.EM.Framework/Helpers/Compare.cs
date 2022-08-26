namespace Eco.EM.Framework.Helpers
{
   
    public static class Compare
    {
        public static bool IsLike(string Value1, string Value2)
        {
            return (Value1.ToLower() == Value2.ToLower());
        }
    }
}
