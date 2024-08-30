using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;

namespace UnionSurvey.Data.Repository
{
    public class SurveyUnionConfigRepo(SurveyUnionContext context)
                    : Repository<SurveyUnionConfig>(context), ISurveyUnionConfigRepo
    {

    }
}
