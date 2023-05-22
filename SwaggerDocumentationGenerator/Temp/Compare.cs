namespace SwaggerDocumentationGenerator.Temp;

public static class Compare
{
    public static IEqualityComparer<TItem> By<TItem, TProperty>(Func<TItem, TProperty> propertyAccessor)
    {
        return new PropertyEqualityComparer<TItem, TProperty>(propertyAccessor);
    }

    public static IEqualityComparer<TItem> By<TItem>(Func<TItem, TItem, bool> comparer)
    {
        return new CustomEqualityComparer<TItem>(comparer);
    }

    private class PropertyEqualityComparer<TItem, TProperty>: CustomEqualityComparer<TItem>
    {
        private readonly Func<TItem, TProperty> propertyAccessor;

        public PropertyEqualityComparer(Func<TItem, TProperty> propertyAccessor)
            : base((item1, item2) => Equals(propertyAccessor(item1), propertyAccessor(item2)))
        {
            this.propertyAccessor = propertyAccessor;
        }

        public override int GetHashCode(TItem obj)
        {
            var propertyValue = propertyAccessor(obj);
            return Equals(propertyValue, default(TItem)) ? 0 : propertyValue.GetHashCode();
        }
    }
}