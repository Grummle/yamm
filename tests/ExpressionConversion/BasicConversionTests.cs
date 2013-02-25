using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;
using tests.Mapping;
using yamm;
using yamm.ExpressionConversion;
using yamm.Mapping;

namespace tests.ExpressionConversion
{
    [TestFixture]
    public class BasicConversionTests
    {
        private Entity _entity;

        [SetUp]
        public void Setup()
        {
            _entity = new Entity{Name="George Michaels"};                        
        }

        [Test]
        public void Should_Convert_Basic_Member_Access()
        {
            var map = new Map(ForType<Entity>.GetProperty(x=>x.Name) , ForType<Model>.GetProperty(x=>x.name));

            Expression<Func<Model, bool>> exp = x => x.name=="George Michaels" ;

            Func<Entity,bool> converted = Converter<Entity>.Convert(exp,new IMap[]{map}).Compile();

            converted(_entity).ShouldBeTrue();
        }
    }
}
