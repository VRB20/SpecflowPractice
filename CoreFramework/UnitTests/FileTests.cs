using CoreFramework.Extensions;
using CoreFramework.Infrastructure.Extensions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

namespace CoreFramework.UnitTests
{
    public class FileTests
    {
        [Test]
        public void CheckInputFileHasData()
        {
            TestDataExtension.TestDataBatchConstruction(Assembly.GetExecutingAssembly());
            var x = "TC001".GetTestData("Product").Trim();
            x.Should().NotBeNull();
        }
    }
}
