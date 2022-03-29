namespace System.HttpProxy
{
    public class DirectResult : ActionResult
    {
        private string _value;

        public DirectResult(string value)
        {
            _value = value;
        }

        public override string ToResponse()
        {
            return _value;
        }
    }
}
