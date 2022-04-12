using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CondoClub.ProxyPagamento {
    public class Excepcoes {

        public class ErroCielo : Exception {

            public ErroCielo() {
            }

            public ErroCielo(string message)
                : base(message) {
            }

        }

    }
}
