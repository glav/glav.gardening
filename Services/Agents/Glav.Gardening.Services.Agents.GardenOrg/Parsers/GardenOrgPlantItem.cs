using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glav.Gardening.Services.Agents.GardenOrg.Parsers
{
    //represents plant information extracted from the GardenOrg site
    public class GardenOrgPlantItem
    {
        public string ScientificName { get; set; }
        public string CommonName { get; set ; }

    }
}
