using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZuegerAdressbook.Model
{
    public class Relationship
    {
        public Person PersonA { get; set; }

        public Person PersonB { get; set; }

        public RelationshipType RelationshipType { get; set; }
    }
}
