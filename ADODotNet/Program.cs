using ADODotNet.Samples;

SqlSamples sqlSamples = new();
//sqlSamples.FirstSample();
// sqlSamples.WorkingWithConnection();
//sqlSamples.ConnectionBuilder();
//sqlSamples.TestReader();
//sqlSamples.TestReaderMultiple();
//sqlSamples.TestReaderMultipleWithProc();
//sqlSamples.AddProduct(1,"S24","The best CPU",999999999);
//sqlSamples.AddProductWithParameter(1,"S25","The best Monitor",998889999);
//sqlSamples.AddTransactional("Digital",4,"Monitor Gigabyte","The best Monitor",123456789);

//sqlSamples.AddBulkInsert();
sqlSamples.AddBulkCopyInsert();