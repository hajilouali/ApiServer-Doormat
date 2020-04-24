using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Services.Bessines
{
    public class AExpertProgres
    {
        private readonly IRepository<Factor> _Factor;
        private readonly IRepository<Product_Factor> _Product_Factor;
        private readonly IRepository<FactorAttachment> _FactorAttachment;
        private readonly IRepository<Manufacture> _Manufacture;
        private readonly IRepository<ProductAndService> _ProductAndService;
        private readonly IRepository<Client> _Client;
        private readonly IRepository<SanadHeading> _SanadHeading;
        private readonly IRepository<Sanad> _Sanad;
        private readonly IRepository<Bank> _Bank;
        private readonly IRepository<AccountingHeading> _AccountingHeading;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<ManufactureHistory> _ManufactureHistory;
        private readonly IRepository<ExpertHistory> _ExpertHistory;

        public AExpertProgres(IRepository<Factor> factor, IRepository<Product_Factor> product_Factor, IRepository<FactorAttachment> factorAttachment, IRepository<Manufacture> manufacture, IRepository<ProductAndService> productAndService, IRepository<Client> client, IRepository<SanadHeading> sanadHeading, IRepository<Sanad> sanad, IRepository<Bank> bank, IRepository<AccountingHeading> accountingHeading, IHostingEnvironment hostingEnvironment, IRepository<ManufactureHistory> manufactureHistory, IRepository<ExpertHistory> expertHistory)
        {
            _Factor = factor;
            _Product_Factor = product_Factor;
            _FactorAttachment = factorAttachment;
            _Manufacture = manufacture;
            _ProductAndService = productAndService;
            _Client = client;
            _SanadHeading = sanadHeading;
            _Sanad = sanad;
            _Bank = bank;
            _AccountingHeading = accountingHeading;
            _hostingEnvironment = hostingEnvironment;
            _ManufactureHistory = manufactureHistory;
            _ExpertHistory = expertHistory;
        }


        public async Task<int> AddPishFactorFromPartner(AddFactorViewModel AddPishFactorViewModel, List<IFormFile> file = null)
        {
            try
            {
                //ذخیره فاکتور
                Factor Factor = new Factor();
                Factor.User_ID = AddPishFactorViewModel.User_ID;
                Factor.FactorType = FactorType.PendingToAccept;
                var client = _Client.GetById(AddPishFactorViewModel.Client_ID);
                Factor.Discription = AddPishFactorViewModel.Discription;
                Factor.Client_ID = client.Id;
                if (AddPishFactorViewModel.rows != null)
                {
                    foreach (var item in AddPishFactorViewModel.rows)
                    {
                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        item.UnitPrice = p.UnitPrice;
                        if (p.UnitType == UnitType.SquareMeters)
                            Factor.TotalPrice += item.Width * item.length * item.Unit * p.UnitPrice;
                        if (p.UnitType == UnitType.Metr)
                            Factor.TotalPrice += item.length * item.Unit * p.UnitPrice;
                        if (p.UnitType == UnitType.Unit)
                            Factor.TotalPrice += item.Unit * p.UnitPrice;

                    }
                }
                Factor.Discount = Factor.TotalPrice * (client.DiscountPercent / 100);
                if (!AddPishFactorViewModel.discountDefoult)
                    Factor.Discount = AddPishFactorViewModel.discoun;

                Factor.FactorPrice = Factor.TotalPrice - Factor.Discount;
                if (AddPishFactorViewModel.Rasmi)
                    Factor.Taxes = Factor.FactorPrice * Convert.ToDecimal(0.09);
                Factor.FactorPrice = Factor.FactorPrice + Factor.Taxes;


                _Factor.Add(Factor);
                //ذخیره ردیف های فاکتور
                List<Product_Factor> listrow = new List<Product_Factor>();
                foreach (var item in AddPishFactorViewModel.rows)
                {
                    var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                    var r = new Product_Factor();

                    r.Factor_ID = Factor.Id;
                    r.length = item.length;
                    r.Width = item.Width;
                    r.Unit = item.Unit;
                    r.UnitPrice = item.UnitPrice;
                    r.RowDiscription = item.RowDiscription;
                    r.ProductAndService_ID = item.ProductAndService_ID;
                    if (p.UnitType == UnitType.SquareMeters)
                        r.Price = (double)item.Width * (double)item.length * item.Unit * item.UnitPrice;
                    if (p.UnitType == UnitType.Metr)
                        r.Price = (double)item.length * item.Unit * item.UnitPrice;
                    if (p.UnitType == UnitType.Unit)
                        r.Price = item.Unit * item.UnitPrice;
                    listrow.Add(r);

                }
                _Product_Factor.AddRange(listrow);
               
                //ذخیره پیوست های فاکتور

                int index = 0;

                bool exists = System.IO.Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + Factor.Id));

                if (!exists)
                    System.IO.Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + Factor.Id));
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + Factor.Id);
                foreach (var item in file)
                {
                    if (CheckContentImage.IsImage(item))
                    {
                        string FName = Factor.Id.ToString() + index.ToString() + Path.GetExtension(item.FileName);
                        var filePath = Path.Combine(uploads, FName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        _FactorAttachment.Add(new FactorAttachment()
                        {
                            FileName = FName,
                            Facor_ID = Factor.Id
                        });
                    }
                }
                return Factor.Id;
            }
            catch
            {

                return 0;
            }
        }
    }
}
