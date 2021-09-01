using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using System.Dynamic;


namespace CampKiosk
{
    public partial class enterPage : Page
    {
        private const string BasePath = "https://campbase-cfe26-default-rtdb.firebaseio.com/";
        private const string FirebaseSecret = "iWFoVTWXtRBJs2HJ6KCVJ5pm0tJG3Qnt3Fq50BEu";
        private static FirebaseClient _client;

        public enterPage()
        {
            InitializeComponent();

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = FirebaseSecret,
                BasePath = BasePath
            };

            _client = new FirebaseClient(config);
        }

        private async void ID_SubmitButton(object sender, RoutedEventArgs e)
        {
            string id;
            string temp;
            id = ID.Text;
            temp = Temp.Text;

            var todoName = new Todo
            {
                tmpName = id,
            };

            var todoTemp = new Todo
            {
                tmpTemp = temp,
            };

            var value1 = await _client.UpdateAsync("tmp", todoName);
            var value2 = await _client.UpdateAsync("tmpTemp", todoTemp);

            var gsh = new GoogleSheetsHelper("kiosksheet-05dfcd0a5ab2.json", "1EJaMVK8ciH1ZtXDXr7chBD1iRZ0r1nrD0QBzhiWxMFI");

            var row1 = new GoogleSheetRow();
            var row2 = new GoogleSheetRow();

            var cell1 = new GoogleSheetCell() { CellValue = "날짜", };
            var cell2 = new GoogleSheetCell() { CellValue = "이름", };
            var cell3 = new GoogleSheetCell() { CellValue = "체온", };

            var cell4 = new GoogleSheetCell() { CellValue = DateTime.Now.ToString("yyyy-MM-dd") };
            var cell5 = new GoogleSheetCell() { CellValue = id };
            var cell6 = new GoogleSheetCell() { CellValue = temp };

            row1.Cells.AddRange(new List<GoogleSheetCell>() { cell1, cell2, cell3 });
            var row = new List<GoogleSheetRow>() { row1 };
            gsh.AddCells(new GoogleSheetParameters() { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = 1 }, row);

            var gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 2, RangeRowEnd = 1000, FirstRowIsHeaders = true, SheetName = "Sheet1" };
            int count = gsh.GetDataRow(gsp);

            row2.Cells.AddRange(new List<GoogleSheetCell>() { cell4, cell5, cell6 });
            var rows = new List<GoogleSheetRow>() { row2 };
            gsh.AddCells(new GoogleSheetParameters() { SheetName = "Sheet1", RangeColumnStart = 1, RangeRowStart = count + 1 }, rows);

            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        internal class Todo
        {
            public string tmpName { get; set; }
            public string tmpTemp { get; set; }
        }
        private async void ID_CheckButton(object sender, RoutedEventArgs e)
        {
            FirebaseResponse response = await _client.GetAsync("tmp/tmpName");
            String value = response.ResultAs<String>();
            MessageBox.Show(value);
            ID_Text.Text = value;
        }

            
        public class GoogleSheetsHelper
        {
            static string[] Scopes = { SheetsService.Scope.Spreadsheets };
            static string ApplicationName = "GoogleSheetsHelper";

            private readonly SheetsService _sheetsService;
            private readonly string _spreadsheetId;

            public GoogleSheetsHelper(string credentialFileName, string spreadsheetId)
            {
                var credential = GoogleCredential.FromStream(new FileStream(credentialFileName, FileMode.Open)).CreateScoped(Scopes);

                _sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                _spreadsheetId = spreadsheetId;

                
            }

            public int GetDataRow(GoogleSheetParameters googleSheetParameters)
            {
                googleSheetParameters = MakeGoogleSheetDataRangeColumnsZeroBased(googleSheetParameters);
                var range = $"{googleSheetParameters.SheetName}!{GetColumnName(googleSheetParameters.RangeColumnStart)}{googleSheetParameters.RangeRowStart}:{GetColumnName(googleSheetParameters.RangeColumnEnd)}{googleSheetParameters.RangeRowEnd}";

                SpreadsheetsResource.ValuesResource.GetRequest request =
                    _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);

                var numberOfColumns = googleSheetParameters.RangeColumnEnd - googleSheetParameters.RangeColumnStart;
                var columnNames = new List<string>();
                var returnValues = new List<ExpandoObject>();

                if (!googleSheetParameters.FirstRowIsHeaders)
                {
                    for (var i = 0; i <= numberOfColumns; i++)
                    {
                        columnNames.Add($"Column{i}");
                    }
                }

                var response = request.Execute();

                int rowCounter = 0;
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        if (googleSheetParameters.FirstRowIsHeaders && rowCounter == 0)
                        {
                            for (var i = 0; i <= numberOfColumns; i++)
                            {
                                columnNames.Add(row[i].ToString());
                            }
                            rowCounter++;
                            continue;
                        }

                        var expando = new ExpandoObject();
                        var expandoDict = expando as IDictionary<String, object>;
                        var columnCounter = 0;
                        foreach (var columnName in columnNames)
                        {
                            expandoDict.Add(columnName, row[columnCounter].ToString());
                            columnCounter++;
                        }
                        returnValues.Add(expando);
                        rowCounter++;
                    }
                }

                return rowCounter;
            }

            public void AddCells(GoogleSheetParameters googleSheetParameters, List<GoogleSheetRow> rows)
            {
                var requests = new BatchUpdateSpreadsheetRequest { Requests = new List<Request>() };

                var sheetId = GetSheetId(_sheetsService, _spreadsheetId, googleSheetParameters.SheetName);

                GridCoordinate gc = new GridCoordinate
                {
                    ColumnIndex = googleSheetParameters.RangeColumnStart - 1,
                    RowIndex = googleSheetParameters.RangeRowStart - 1,
                    SheetId = sheetId
                };

                var request = new Request { UpdateCells = new UpdateCellsRequest { Start = gc, Fields = "*" } };

                var listRowData = new List<RowData>();

                foreach (var row in rows)
                {
                    var rowData = new RowData();
                    var listCellData = new List<CellData>();
                    foreach (var cell in row.Cells)
                    {
                        var cellData = new CellData();
                        var extendedValue = new ExtendedValue { StringValue = cell.CellValue };

                        cellData.UserEnteredValue = extendedValue;
                        var cellFormat = new CellFormat { TextFormat = new TextFormat() };

                        if (cell.IsBold)
                        {
                            cellFormat.TextFormat.Bold = true;
                        }



                        cellData.UserEnteredFormat = cellFormat;
                        listCellData.Add(cellData);
                    }
                    rowData.Values = listCellData;
                    listRowData.Add(rowData);
                }
                request.UpdateCells.Rows = listRowData;

                // It's a batch request so you can create more than one request and send them all in one batch. Just use reqs.Requests.Add() to add additional requests for the same spreadsheet
                requests.Requests.Add(request);

                _sheetsService.Spreadsheets.BatchUpdate(requests, _spreadsheetId).Execute();
            }

            private string GetColumnName(int index)
            {
                const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var value = "";

                if (index >= letters.Length)
                    value += letters[index / letters.Length - 1];

                value += letters[index % letters.Length];
                return value;
            }

            private GoogleSheetParameters MakeGoogleSheetDataRangeColumnsZeroBased(GoogleSheetParameters googleSheetParameters)
            {
                googleSheetParameters.RangeColumnStart = googleSheetParameters.RangeColumnStart - 1;
                googleSheetParameters.RangeColumnEnd = googleSheetParameters.RangeColumnEnd - 1;
                return googleSheetParameters;
            }

            private int GetSheetId(SheetsService service, string spreadSheetId, string spreadSheetName)
            {
                var spreadsheet = service.Spreadsheets.Get(spreadSheetId).Execute();
                var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == spreadSheetName);
                int sheetId = (int)sheet.Properties.SheetId;
                return sheetId;
            }
        }

        public class GoogleSheetCell
        {
            public string CellValue { get; set; }
            public bool IsBold { get; set; }

        }

        public class GoogleSheetParameters
        {
            public int RangeColumnStart { get; set; }
            public int RangeRowStart { get; set; }
            public int RangeColumnEnd { get; set; }
            public int RangeRowEnd { get; set; }
            public string SheetName { get; set; }
            public bool FirstRowIsHeaders { get; set; }
        }

        public class GoogleSheetRow
        {
            public GoogleSheetRow() => Cells = new List<GoogleSheetCell>();

            public List<GoogleSheetCell> Cells { get; set; }
        }
    }
}

