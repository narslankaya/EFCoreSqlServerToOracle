using EfTestApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EfTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var cb = new Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder
            {
                DataSource = "localhost:1521/orcl",
                UserID = "scott",
                Password = "tiger"
            };

            var dbcob = new DbContextOptionsBuilder<MyDbContext>();
            dbcob.UseOracle(cb.ToString(), s =>
            {
                s.UseOracleSQLCompatibility("11");
            });

            var db = new MyDbContext(dbcob.Options);

            var sb = db.Database.GenerateCreateScript();
            System.IO.File.WriteAllText("TableCreationInOracle.sql", sb);

        }
    }

    namespace Model
    {
        // her defasında yeniden yazmak gerekmesin diye partial class buraya gerekli 
        // constructor ile taşındı...

        public partial class TestDbContext
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }
        }

        // biz testdbcontext değil myDbContext'i kullanacağız. 
        public class MyDbContext : TestDbContext
        {
            public MyDbContext()
            {
            }

            public MyDbContext(DbContextOptions<MyDbContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                if (this.Database.ProviderName == "Oracle.EntityFrameworkCore")
                {

                    //modelBuilder.Entity<Bu_Benim_Uzun_Isimli_Tablom>().ToTable("UZUNISIMLITABLO");

                    // ileride kullanmak üzere converter oluşturuyoruz.
                    var guidMapper = new GuidToStringConverter(new ConverterMappingHints(unicode: false));

                    //tabloları listeliyoruz.
                    foreach (var table in modelBuilder.Model.GetEntityTypes())
                    {

                        // tablo adı 30 karakteri geçiyor mu?
                        if (table.FullName().Length > 30)
                        {
                            var tblName = table.FullName()[..30];
                            table.SetOrRemoveAnnotation("Relational:TableName", tblName);
                        }

                        // tablo adı etiketi konduysa ann dolu gelir.
                        var ann = table.FindAnnotation("Relational:TableName");
                        if (ann != null)
                        {
                            var name = ann.Value.ToString().ToUpperInvariant();
                            table.RemoveAnnotation("Relational:TableName");
                            table.AddAnnotation("Relational:TableName", name);
                        }
                        else
                        {
                            table.AddAnnotation("Relational:TableName", table.FullName().ToUpperInvariant());
                            table.SetTableName(table.FullName().ToUpperInvariant());
                        }

                        // alanları tarıyoruz. Sadece alanlar değil aynı zamanda navigation property'leri de geliyor. Buraya dikkat.
                        foreach (var column in table.GetProperties())
                        {
                            if (column.Name.Length > 30)
                            {
                                var colName = column.Name[..30];
                                column.SetOrRemoveAnnotation("Relational:ColumnName", colName);
                            }

                            // alan adı etiketi varsa colAnn dolu gelir.
                            var colAnn = column.FindAnnotation("Relational:ColumnName");
                            if (colAnn != null)
                            {
                                var colName = colAnn.Value.ToString().ToUpperInvariant();
                                column.RemoveAnnotation("Relational:ColumnName");
                                column.AddAnnotation("Relational:ColumnName", colName);
                                column.SetColumnName(colName);
                            }
                            else
                            {
                                column.AddAnnotation("Relational:ColumnName", column.Name.ToUpperInvariant());
                                column.SetColumnName(column.Name.ToUpperInvariant());
                            }

                            // Guid ve Guid? tipi alanları kontrol ediyoruz.
                            if (column.PropertyInfo != null && (column.PropertyInfo.PropertyType.Name == "Guid" ||
                                column.PropertyInfo.PropertyType.IsGenericType &&
                                column.PropertyInfo.PropertyType.GenericTypeArguments[0].Name == "Guid"))
                            {
                                var annConv = column.FindAnnotation("ValueConverter");
                                if (annConv == null)
                                {
                                    column.AddAnnotation("ValueConverter", guidMapper);
                                }
                            }

                            // datetime tipindeki alanları kontrol ediyoruz.
                            var annDataType = column.FindAnnotation("Relational:ColumnType");
                            if (annDataType != null && annDataType.Value.ToString() == "datetime")
                            {
                                column.SetOrRemoveAnnotation("Relational:ColumnType", "TIMESTAMP(6)");
                            }
                        }

                        // aşağıdaki kısım indeks ve fk isimlerini kırpmak için eklendi. 
                        foreach (var index in table.GetIndexes())
                        {
                            if (!string.IsNullOrEmpty(index.GetDatabaseName()))
                            {
                                var ixName = index.GetDatabaseName().ToUpperInvariant();
                                if (ixName.Length > 30) ixName = ixName.Substring(0, 30);
                                index.SetDatabaseName(ixName);
                            }
                        }

                        foreach (var fk in table.GetForeignKeys())
                        {
                            var fkName = fk.GetConstraintName();
                            if (!string.IsNullOrEmpty(fkName))
                            {
                                fkName = fkName.ToUpperInvariant();
                                if (fkName.Length > 30) fkName = fkName.Substring(0, 30);
                                fk.SetConstraintName(fkName);
                            }
                        }
                    }
                }
            }
        }

    }
}