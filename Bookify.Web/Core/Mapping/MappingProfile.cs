namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Category
            CreateMap<Category, CategoryViewModel>();                   //Category to CategoryViewModel
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();  //CategoryFormViewModel to Category

            //Author
            CreateMap<Author, AuthorViewModel>();                       //Author to AuthorViewModel
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();      //AuthorFormViewModel to Author


        }
    }
}
