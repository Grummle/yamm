using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;


namespace yamm
{

    public abstract class ModelMap<TEntity, TModel>
    {
        public IList<PropertyInfo> Properties = new List<PropertyInfo>();

        public ModelMap<TEntity, TModel> Property(Expression<Func<TModel, object>> map)
        {
            var property = ForType<TModel>.GetProperty(map);// typeof(TModel).GetProperty(map);
            Properties.Add(property);
            return this;
        }
    }

}
