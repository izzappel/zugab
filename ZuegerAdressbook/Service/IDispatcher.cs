using System;

namespace ZuegerAdressbook.Service
{
    public interface IDispatcher
    {
        void Dispatch(Delegate method);
    }
}