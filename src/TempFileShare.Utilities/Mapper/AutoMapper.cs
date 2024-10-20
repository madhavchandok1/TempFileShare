using AutoMapper;
using TempFileShare.Contracts.DataTransferObjects.Sessions;
using TempFileShare.Contracts.Models;

namespace TempFileShare.Utilities.Mapper
{
    public static class AutoMap
    {
        public static readonly IMapper Mapper;

        static AutoMap()
        {
            MapperConfiguration config = new(cfg =>
            {
                // Create the map between Files and DBFilesDetails
                _ = cfg.CreateMap<Files, DBFilesDetails>();
            });
            Mapper = config.CreateMapper();
        }

        public static DBFilesDetails MapToDBFilesDetails(Files file)
        {
            return Mapper.Map<DBFilesDetails>(file);
        }
    }
}
