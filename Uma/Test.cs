using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;
using Autodesk.Revit.DB;
using Revit.Elements;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Autodesk.Revit.DB.PointClouds;

namespace Revit

{
    //Perdon francisco hoy codie pero termine los ejemplos del gallego =) 
    public class Test
        
    {
        public static string EsMuroCortina(Revit.Elements.Element elemento)
        {
            if (elemento.GetCategory.ToString() != "Walls")
            {
                return elemento.Id + "It s not a wall";

            }
            else
            {
                //hago el transpaso --- El internal element es el unwrap
                Autodesk.Revit.DB.Element elementoRevit = elemento.InternalElement;
                //casteo a un wall de revit
                Autodesk.Revit.DB.Wall muroRevit = (Autodesk.Revit.DB.Wall) elementoRevit;

                //Los separo segun tengan o no grid
                CurtainGrid tieneGrid = muroRevit.CurtainGrid;

                if (tieneGrid == null)
                {
                    return muroRevit.Id + " It is not a Curtain Wall";
                }

                else 
                {
                    return muroRevit.Id + "It is a Curtain Wall";
                }
            }
        }

    }
}
