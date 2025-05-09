using System.Reflection;
using Xunit.Sdk;

namespace SocialMedia.TestUtilities
{
    public class UseMappingContextScopeAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            MappingContextScope.Current = new MappingContext();
        }

        public override void After(MethodInfo methodUnderTest)
        {
            MappingContextScope.Reset();
        }
    }
}
