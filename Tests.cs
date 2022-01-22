using System.Linq;
using System;
using System.Collections.Generic;

using NUnit.Framework;
using AutoFixture;
using Moq;

namespace Interview
{
    [TestFixture]
    public class Tests
    {
        private SimpleRepository<TestModel> repository;
        private IFixture fixture = new Fixture();
        private Mock<IDataProvider<TestModel>> dataProviderMock;
        private Random random = new Random();

        [SetUp]
        public void Setup()
        {          
            dataProviderMock = new Mock<IDataProvider<TestModel>>();
            dataProviderMock.Setup(c => c.LoadData());
        }

        #region Ctor
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.All))]
        public void Ctor_Creates_An_Empty_Collection_If_DataProvider_Returns_Null()
        {
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => null);
            Assert.DoesNotThrow(() => new SimpleRepository<TestModel>(dataProviderMock.Object));
        }

        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.All))]
        public void Ctor_Creates_A_Collection_Containing_All_Items_Returned_By_Data_Provider()
        {
            var data = GetSampleData(fixture.Create<byte>());
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);
            SimpleRepository<TestModel> repo = null;
            Assert.DoesNotThrow(() => repo = new SimpleRepository<TestModel>(dataProviderMock.Object));
            dataProviderMock.Verify(c => c.LoadData());
            Assert.AreEqual(data.Count(), repo.Data.Count());
        }
        #endregion

        #region All
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(200)]
        [TestCase(562)]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.All))]
        public void All_Returns_A_Collection_Of_All_Items_In_Local_Store(int itemCount)
        {
            var data = GetSampleData(itemCount);
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            
            var result = repository.All();
            dataProviderMock.Verify(c => c.LoadData());

            Assert.AreEqual(itemCount, result.Count());
        }
        #endregion

        #region Delete
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Delete))]
        public void Delete_Throws_An_ArgumentNullException_When_Id_Supplied_Is_Null()
        {
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            Assert.Throws<ArgumentNullException>(() => repository.Delete(null));
        }
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Delete))]
        public void Delete_Throws_An_ArgumentException_When_Id_Supplied_Is_String_And_Whitespace_Or_Empty()
        {
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            Assert.Throws<ArgumentException>(() => repository.Delete(string.Empty));
            Assert.Throws<ArgumentException>(() => repository.Delete("   "));
        }
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Delete))]
        public void Delete_Throws_An_ItemNotFoundException_When_Id_Supplied_Is_Not_Matching_An_Existing_Item()
        {
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => GetSampleData(fixture.Create<byte>()));
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);

            var testGuid = Guid.NewGuid();
            Assert.Throws<ItemNotFoundException>(
                () => repository.Delete(testGuid),
                testGuid.ToString(), testGuid);
        }

        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Delete))]
        public void Delete_Removes_Matching_Item_For_Id_Supplied()
        {
            var data = GetSampleData(fixture.Create<int>());

            var initialCount = data.Count();
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);

            var testGuid = GetRandomItem(data).Id;

            Assert.DoesNotThrow(() => repository.Delete(testGuid));
            Assert.Greater(initialCount, repository.Data.Count());
        }

        #endregion

        #region FindById
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.FindById))]
        public void FindById_Throws_An_ArgumentNullException_When_Id_Supplied_Is_Null()
        {
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            Assert.Throws<ArgumentNullException>(() => repository.FindById(null));
        }
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.FindById))]
        public void FindById_Throws_An_ArgumentException_When_Id_Supplied_Is_String_And_Whitespace_Or_Empty()
        {
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            Assert.Throws<ArgumentException>(() => repository.FindById(string.Empty));
            Assert.Throws<ArgumentException>(() => repository.FindById("   "));
        }

        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.FindById))]
        public void FindById_Returns_Null_When_Supplied_Id_Does_Not_Match_To_An_Existing_Item()
        {
            var data = GetSampleData(fixture.Create<byte>());
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);

            var guid = Guid.NewGuid();
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            dataProviderMock.Verify(c => c.LoadData());
            TestModel result = null;
            Assert.DoesNotThrow(() =>
            {
                result = repository.FindById(guid);
            });
            Assert.IsNull(result);
        }

        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.FindById))]
        public void FindById_Returns_Item_When_Supplied_Id_Matches()
        {
            var data = GetSampleData(fixture.Create<byte>());
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);

            var item = GetRandomItem(data);
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            dataProviderMock.Verify(c => c.LoadData());
            TestModel result = null;
            Assert.DoesNotThrow(() =>
            {
                result = repository.FindById(item.Id);
            });
            Assert.IsNotNull(result);
        }
        #endregion

        #region Save
        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Save))]
        public void Save_Throws_ArgumentNullException_When_Supplied_Value_Is_Null()
        {
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);
            Assert.Throws<ArgumentNullException>(() => repository.Save(null));
        }

        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Save))]
        public void Save_Updates_Existing_Entry_If_Item_Is_Found_In_Collection()
        {
            var data = GetSampleData(fixture.Create<int>());
            const string newName = "A_New_Name_To_Use";
            var initialCount = data.Count();
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);

            var item = GetRandomItem(data);
            item.Name = newName;

            Assert.DoesNotThrow(() => repository.Save(item));
            repository.Data.TryGetValue(item.Id, out TestModel result);

            Assert.AreEqual(initialCount, repository.Data.Count());
            Assert.AreEqual(newName, result.Name);
        }

        [Test]
        [Category(nameof(SimpleRepository<TestModel>))]
        [Category(nameof(SimpleRepository<TestModel>.Save))]
        public void Save_Creates_New_Entry_If_Item_Is_Not_Found_In_Collection()
        {
            var data = GetSampleData(fixture.Create<int>());
            const string newName = "SomeNameForTheEntity";
            var initialCount = data.Count();
            dataProviderMock.Setup(c => c.LoadData()).Returns(() => data);
            repository = new SimpleRepository<TestModel>(dataProviderMock.Object);

            var item = new TestModel() { Id = Guid.NewGuid(), Name = newName };

            Assert.DoesNotThrow(() => repository.Save(item));
            repository.Data.TryGetValue(item.Id, out TestModel result);

            Assert.Greater(repository.Data.Count(), initialCount);
            Assert.AreEqual(newName, result.Name);
        }
        #endregion

        #region Utilities
        private IEnumerable<TestModel> GetSampleData(int count)
        {
            return TestModel.Generate(fixture, count);
        }

        private TestModel GetRandomItem(IEnumerable<TestModel> collection)
        {
            var count = random.Next(0, collection.Count() - 1);
            return collection.Skip(count - 1).Take(1).Single();
        }
        #endregion

    }

    public class TestModel : IStoreable
    {
        public IComparable Id { get; set; }
        public string Name { get; set; }

        public static IEnumerable<TestModel> Generate(IFixture fixture, int count)
        {
            var result = new List<TestModel>();
            var counter = 0;
            while (counter < count)
            {
                result.Add(new TestModel() { Id = Guid.NewGuid(), Name = fixture.Create<string>() });
                counter += 1;
            }
            return result;
        }
    }
}