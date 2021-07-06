using System.Collections.Generic;

namespace WebProductNotifier.Classes
{
    public interface SearchInterface
    {
        
        public List<ProductObject> searchProduct(ObjectSearch objectSearch);
        public ShopObject Shop();

        public ProductFullInformationObject getProductFullInformationObject(ObjectToSearch objectToSearch);
    }
}