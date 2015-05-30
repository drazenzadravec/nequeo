using System;

namespace Nequeo.Cryptography.Key.Pkix
{
    public class PkixNameConstraintValidatorException : Exception
    {
        public PkixNameConstraintValidatorException(String msg)
            : base(msg)
        {
        }
    }
}
