using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deedle;
using Newtonsoft.Json;

namespace EikonDataAPI
{
    public partial class DataGrid : EndPoint,  IDataGrid
    {
        private Frame<int, string> CreateFrame(DataResponse response)
        {
            if (response != null)
            {
                var rows = Enumerable.Range(0, response.data.Count).Select(i =>
                {
                    // Build each row using series builder & return 
                    // KeyValue representing row key with row data
                    var sb = new SeriesBuilder<string>();
                    for (int j = 0; j < response.headers.First().Count(); j++)
                    {
                        string displayName;
                        if (string.IsNullOrEmpty(response.headers.First()[j].displayName))
                        {
                            displayName = "None";
                        }
                        else
                        {
                            displayName = response.headers.First()[j].displayName;
                        }

                        sb.Add(displayName, response.data[i][j].Value);



                    }


                    return KeyValue.Create(i, sb.Series);
                });

                return Frame.FromRows(rows);
            }
            else
            {
                return Frame.CreateEmpty<int, string>();
            }
        }

        //public static Frame<int, string> GetDataFrame(DataResponse response)
        //{
        //    CreateLogger(eikon);
        //    if (response == null) return Frame.CreateEmpty<int, string>();
        //    else return CreateFrame(response);
        //}
        public Frame<int, string> GetData(string instrument,
            string field,
            // DataGridParameters parameters,
            Dictionary<string, string> parameters = null)
        {

            var response = GetDataRaw(instrument, field, parameters);


            return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }

        public Frame<int, string> GetData(IEnumerable<string> instruments,
    IEnumerable<TRField> fields,
    // DataGridParameters parameters,
    Dictionary<string, string> parameters = null)
        {

            var response = GetDataRaw(instruments, fields, parameters);
            return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));

        }

        public Frame<int, string> GetData(IEnumerable<string> instruments,
            IEnumerable<string> fields,
            //DataGridParameters parameters,
            Dictionary<string, string> parameters = null)
        {

            var response = GetDataRaw(instruments, fields, parameters);
            return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }
        public Frame<int, string> GetData(string instrument,
            IEnumerable<TRField> fields,
            //DataGridParameters parameters,
            Dictionary<string, string> parameters = null)
        {

            var response = GetDataRaw(instrument, fields, parameters);
            return CreateFrame(JsonConvert.DeserializeObject<DataResponse>(response, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            }));
        }
    }
}
