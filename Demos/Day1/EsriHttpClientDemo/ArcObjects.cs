using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsriHttpClientDemo {
    /// <summary>
    /// Copied from 
    /// <a href="http://gis.stackexchange.com/questions/14859/arcobjects-adding-arcgis-server-layer-to-mxd-programtically"></a>
    /// </summary>
    internal class ArcObjects
    {
        protected void OnClick()
        {
            try
            {
                string url =
                    "http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer";

                var visIndicies = new List<int>();
                visIndicies.Add(2); // just make the counties visible
                var layer = GetArcGISMapServiceLayer("States", visIndicies, url, 50, false);
                if (layer != null)
                    ArcMap.Document.FocusMap.AddLayer(layer);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                while (ex != null)
                {
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.StackTrace);
                    ex = ex.InnerException;
                }
                MessageBox.Show(sb.ToString());
            }
        }


        public static ILayer GetArcGISMapServiceLayer(string name, List<int> visibleIndices, string url,
            short transparency, bool isTiled)
        {
            ILayer outLayer = null;
            string svcName = GetServiceName(url);
            string svcUrl = GetServiceUrl(url);
            var gisServer = OpenConnection(svcUrl);

            var soName = FindServerObjectname(gisServer, svcName);
            if (soName == null)
                throw new Exception("unable to find serverobject for " + svcName);

            outLayer = GetArcGisServerGroupLayer(soName) as ILayer;
            if (outLayer != null)
            {
                if (!isTiled)
                {
                    var grpLayer = outLayer as ICompositeLayer;
                    for (int i = 0; i < grpLayer.Count; i++)
                    {
                        Debug.Print("Setting visibility for " + grpLayer.get_Layer(i).Name);
                        SetVisibility(grpLayer.get_Layer(i), visibleIndices);
                    }
                }
                Debug.Print("setting {0} transparency to {1}", name, transparency);
                ((ILayerEffects) outLayer).Transparency = transparency;
                outLayer.Name = name;
            }
            return outLayer;
        }

        private static IMapServerLayer GetArcGisServerGroupLayer(IAGSServerObjectName3 soName)
        {
            IMapServerLayer outLayer = null;
            var factory = new MapServerLayerFactory() as ILayerFactory;
            try
            {
                //create an enum of layers using the name object (will contain only a single layer)
                outLayer = factory.Create(soName).Next() as IMapServerLayer;
            }
            catch
            {
                throw;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(factory);
            }
            return outLayer;
        }

        private static IAGSServerObjectName3 FindServerObjectname(IAGSServerConnection gisServer, string svcName)
        {
            var soNames = gisServer.ServerObjectNames;
            IAGSServerObjectName3 soName;
            while ((soName = (IAGSServerObjectName3) soNames.Next()) != null)
            {
                Debug.Print("soName: " + soName.Name);
                if ((soName.Type == "MapServer") && (soName.Name.ToUpper() == svcName.ToUpper()))
                {
                    return soName;
                }
            }
            return null;
        }

        private static IAGSServerConnection OpenConnection(string svcUrl)
        {
            //create a property set to hold connection properties
            var connectionProps = new PropertySet() as IPropertySet;
            //specify the URL for the server
            connectionProps.SetProperty("URL", svcUrl); // layerDefinition.Url);
            //define username and password for the connection
            //connectionProps.SetProperty("USER", "<USER>");
            //connectionProps.SetProperty("PASSWORD", "<PASS>");
            //open the server connection, pass in the property set, get a connection object back

            //create a new ArcGIS Server connection factory
            var connectionFactory = (IAGSServerConnectionFactory2) new AGSServerConnectionFactory();
            var gisServer = connectionFactory.Open(connectionProps, 0);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(connectionFactory);
            return gisServer;
        }

        private static void SetVisibility(ILayer lyr, List<int> visibleIds)
        {
            // recurse and set visibility
            IMapServerSublayer subLayer = lyr as IMapServerSublayer;
            if (subLayer == null)
                return;
            int id = subLayer.LayerDescription.ID;

            if (visibleIds.Contains(id))
                lyr.Visible = true;
            else
                lyr.Visible = false;
            IMapServerGroupLayer gLayer = lyr as IMapServerGroupLayer;
            if (gLayer != null)
            {
                for (int i = 0; i < gLayer.Count; i++)
                    SetVisibility(gLayer.get_Layer(i), visibleIds);
            }
        }

        private static string GetServiceUrl(string url)
        {
            // remove the "rest" part of the url 
            int idx = url.ToString().ToUpper().IndexOf(@"/REST/");
            string svcUrl = url.Substring(0, idx) + @"/services";
            return svcUrl;
        }

        private static string GetServiceName(string url)
        {
            int idx = url.ToString().ToUpper().IndexOf(@"/SERVICES/") + 10;
            string svcName = url.ToString().Substring(idx).Trim();
            if (svcName.ToUpper().EndsWith(@"/MAPSERVER"))
            {
                svcName = svcName.Substring(0, svcName.ToUpper().LastIndexOf(@"/MAPSERVER"));
            }
            return svcName;
        }
    }
}

