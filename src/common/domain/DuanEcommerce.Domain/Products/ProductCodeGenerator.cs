using DuanEcommerce.IdentitySettings;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace DuanEcommerce.Products;

public class ProductCodeGenerator(IRepository<IdentitySetting> identitySettingRepository) : ITransientDependency
{
    private readonly IRepository<IdentitySetting> _identitySettingRepository = identitySettingRepository;
    public async Task<string> GenerateAsync()
    {
        string newCode;
        var identitySetting = await _identitySettingRepository.FindAsync(x => x.Id == DuanEcommerceConsts.ProductIdentitySettingId);
        if (identitySetting == null)
        {
            identitySetting = await _identitySettingRepository.InsertAsync(new IdentitySetting(DuanEcommerceConsts.ProductIdentitySettingId, "Sản phẩm", DuanEcommerceConsts.ProductIdentitySettingPrefix, 1, 1));
            newCode = identitySetting.Prefix + identitySetting.CurrentNumber;

        }
        else
        {
            identitySetting.CurrentNumber += identitySetting.StepNumber;
            newCode = identitySetting.Prefix + identitySetting.CurrentNumber;

            await _identitySettingRepository.UpdateAsync(identitySetting);
        }
        return newCode;
    }
}
