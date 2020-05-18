using Common;
using Data.Repositories;
using Entities;
using Entities.Accounting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Services.Services;
using Services.ViewModels.Requests;
using Services.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace Services.Bessines
{
    public class AccountingProgres
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
        private readonly IRepository<SanadAttachment> _SanadAttachment;
        private readonly SiteSettings _siteSetting;
        public AccountingProgres(SiteSettings settings, IRepository<Factor> factor, IRepository<Product_Factor> product_Factor, IRepository<FactorAttachment> factorAttachment, IRepository<Manufacture> manufacture, IRepository<ProductAndService> productAndService, IRepository<Client> client, IRepository<SanadHeading> sanadHeading, IRepository<Sanad> sanad, IRepository<Bank> bank, IRepository<AccountingHeading> accountingHeading, IHostingEnvironment hostingEnvironment, IRepository<ManufactureHistory> manufactureHistory, IRepository<ExpertHistory> expertHistory, IRepository<SanadAttachment> sanadAttachment)
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
            _SanadAttachment = sanadAttachment;
            _siteSetting = settings;
        }
        #region Factor
        public async Task<int> AddCellsPartnerFactor(AddFactorPartnerViewModel AddFactorViewModel, List<IFormFile> file = null)
        {
            try
            {
                //ذخیره فاکتور
                Factor Factor = new Factor();
                Factor.Discription = AddFactorViewModel.Discription;
                Factor.User_ID = AddFactorViewModel.UserID;
                Factor.FactorType = FactorType.PendingToAccept;
                var client = _Client.TableNoTracking.Where(p => p.User_ID == AddFactorViewModel.UserID).FirstOrDefault();
                Factor.Client_ID = client.Id;
                if (AddFactorViewModel.rows != null)
                {
                    foreach (var item in AddFactorViewModel.rows)
                    {

                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        item.UnitPrice = p.UnitPrice;
                        if (p.UnitType == UnitType.SquareMeters)
                            Factor.TotalPrice += item.Width * item.length * item.Unit * (decimal)item.UnitPrice;
                        if (p.UnitType == UnitType.Metr)
                            Factor.TotalPrice += item.length * item.Unit * (decimal)item.UnitPrice;
                        if (p.UnitType == UnitType.Unit)
                            Factor.TotalPrice += item.Unit * (decimal)item.UnitPrice;

                    }
                }

                Factor.Discount = Factor.TotalPrice * (client.DiscountPercent / 100);
                Factor.FactorPrice = Factor.TotalPrice - Factor.Discount;
                _Factor.Add(Factor);
                //ارسال اس ام اس 
                if (!string.IsNullOrEmpty(client.ClientPhone))
                {
                    try
                    {
                        SMSManager sMSManager = new SMSManager(_siteSetting.SMSConfiguration.ApiKey, _siteSetting.SMSConfiguration.SecurityCode);
                        SmsIrRestful.UltraFastParameters[] paramert =
                        {
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "UserName",
                    ParameterValue = client.ClientName
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "FactorType",
                    ParameterValue = "پیش فاکتور"
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "FactorNumber",
                    ParameterValue = Factor.Id.ToString()
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Price",
                    ParameterValue = Factor.FactorPrice.ToString("n0")+"ريال"
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Description",
                    ParameterValue = Factor.FactorCodeView
                }
                };
                        var mobile = long.Parse(client.ClientPhone);
                        var dto = new SmsIrRestful.UltraFastSend()
                        {
                            Mobile = mobile,
                            ParameterArray = paramert,
                            TemplateId = _siteSetting.SMSConfiguration.FactorThemplateID
                        };
                        var sms = sMSManager.VerificationCodeByThemplate(dto);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                //ذخیره ردیف های فاکتور
                List<Product_Factor> listrow = new List<Product_Factor>();
                foreach (var item in AddFactorViewModel.rows)
                {
                    var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                    var r = new Product_Factor();

                    r.Factor_ID = Factor.Id;
                    r.length = item.length;
                    r.Width = item.Width;
                    r.Unit = item.Unit;
                    r.UnitPrice = item.UnitPrice;
                    r.ProductAndService_ID = item.ProductAndService_ID;
                    if (p.UnitType == UnitType.SquareMeters)
                        r.Price = (double)(item.Width * item.length * item.Unit * (decimal)item.UnitPrice);
                    if (p.UnitType == UnitType.Metr)
                        r.Price = (double)(item.length * item.Unit * (decimal)item.UnitPrice);
                    if (p.UnitType == UnitType.Unit)
                        r.Price = item.Unit * item.UnitPrice;
                    r.RowDiscription = item.RowDiscription;
                    listrow.Add(r);

                }
                _Product_Factor.AddRange(listrow);
                //ذخیره پیوست های فاکتور

                if (file != null && file.Count > 0)
                {
                    int index = 0;

                    bool exists = Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + Factor.Id));

                    if (!exists)
                        Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + Factor.Id));
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
                }
                return Factor.Id;
            }
            catch
            {

                return 0;
            }

        }
        public async Task<int> AddPishFactor(AddFactorViewModel AddPishFactorViewModel, List<IFormFile> file = null)
        {
            try
            {
                //ذخیره فاکتور
                Factor Factor = new Factor();
                Factor.Discription = AddPishFactorViewModel.Discription;
                Factor.User_ID = AddPishFactorViewModel.User_ID;
                Factor.FactorType = FactorType.PishFactor;
                var client = _Client.GetById(AddPishFactorViewModel.Client_ID);
                Factor.Client_ID = client.Id;
                if (AddPishFactorViewModel.rows != null)
                {
                    foreach (var item in AddPishFactorViewModel.rows)
                    {

                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        item.UnitPrice = p.UnitPrice;
                        if (p.UnitType == UnitType.SquareMeters)
                            Factor.TotalPrice += item.Width * item.length * item.Unit * (decimal)item.UnitPrice;
                        if (p.UnitType == UnitType.Metr)
                            Factor.TotalPrice += item.length * item.Unit * (decimal)item.UnitPrice;
                        if (p.UnitType == UnitType.Unit)
                            Factor.TotalPrice += item.Unit * (decimal)item.UnitPrice;

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
                    r.ProductAndService_ID = item.ProductAndService_ID;
                    if (p.UnitType == UnitType.SquareMeters)
                        r.Price = (double)(item.Width * item.length * item.Unit * (decimal)item.UnitPrice);
                    if (p.UnitType == UnitType.Metr)
                        r.Price = (double)(item.length * item.Unit * (decimal)item.UnitPrice);
                    if (p.UnitType == UnitType.Unit)
                        r.Price = item.Unit * item.UnitPrice;
                    r.RowDiscription = item.RowDiscription;
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
                //ارسال اس ام اس 
                if (!string.IsNullOrEmpty(client.ClientPhone))
                {
                    try
                    {
                        SMSManager sMSManager = new SMSManager(_siteSetting.SMSConfiguration.ApiKey, _siteSetting.SMSConfiguration.SecurityCode);
                        SmsIrRestful.UltraFastParameters[] paramert =
                        {
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "UserName",
                    ParameterValue = client.ClientName
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "FactorType",
                    ParameterValue = "پیش فاکتور"
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "FactorNumber",
                    ParameterValue = Factor.Id.ToString()
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Price",
                    ParameterValue = Factor.FactorPrice.ToString("n0")+"ريال"
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Description",
                    ParameterValue = Factor.FactorCodeView
                }
                };
                        var mobile = long.Parse(client.ClientPhone);
                        var dto = new SmsIrRestful.UltraFastSend()
                        {
                            Mobile = mobile,
                            ParameterArray = paramert,
                            TemplateId = _siteSetting.SMSConfiguration.FactorThemplateID
                        };
                        var sms = sMSManager.VerificationCodeByThemplate(dto);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return Factor.Id;
            }
            catch
            {

                return 0;
            }
        }
        public async Task<int> AddFactor(AddFactorViewModel AddFactorViewModel, List<IFormFile> file = null)
        {
            try
            {
                //ذخیره فاکتور
                Factor Factor = new Factor();
                Factor.Discription = AddFactorViewModel.Discription;
                Factor.User_ID = AddFactorViewModel.User_ID;
                Factor.FactorType = FactorType.Factor;
                var client = _Client.GetById(AddFactorViewModel.Client_ID);
                Factor.Client_ID = client.Id;
                if (AddFactorViewModel.rows != null)
                {
                    foreach (var item in AddFactorViewModel.rows)
                    {

                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        item.UnitPrice = p.UnitPrice;
                        if (p.UnitType == UnitType.SquareMeters)
                            Factor.TotalPrice += item.Width * item.length * item.Unit * (decimal)item.UnitPrice;
                        if (p.UnitType == UnitType.Metr)
                            Factor.TotalPrice += item.length * item.Unit * (decimal)item.UnitPrice;
                        if (p.UnitType == UnitType.Unit)
                            Factor.TotalPrice += item.Unit * (decimal)item.UnitPrice;

                    }
                }

                Factor.Discount = Factor.TotalPrice * (client.DiscountPercent / 100);
                if (!AddFactorViewModel.discountDefoult)
                    Factor.Discount = AddFactorViewModel.discoun;
                Factor.FactorPrice = Factor.TotalPrice - Factor.Discount;
                if (AddFactorViewModel.Rasmi)
                    Factor.Taxes = Factor.FactorPrice * Convert.ToDecimal(0.09);
                Factor.FactorPrice = Factor.FactorPrice + Factor.Taxes;
                _Factor.Add(Factor);
                //ذخیره ردیف های فاکتور
                List<Product_Factor> listrow = new List<Product_Factor>();
                foreach (var item in AddFactorViewModel.rows)
                {
                    var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                    var r = new Product_Factor();
                    r.RowDiscription = item.RowDiscription;
                    r.Factor_ID = Factor.Id;
                    r.length = item.length;
                    r.Width = item.Width;
                    r.Unit = item.Unit;
                    r.UnitPrice = item.UnitPrice;
                    r.ProductAndService_ID = item.ProductAndService_ID;
                    if (p.UnitType == UnitType.SquareMeters)
                        r.Price = (double)(item.Width * item.length * item.Unit * (decimal)item.UnitPrice);
                    if (p.UnitType == UnitType.Metr)
                        r.Price = (double)(item.length * item.Unit * (decimal)item.UnitPrice);
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
                //ایجاد ردیف تولید
                Manufacture manufacture = new Manufacture()
                {
                    Factor_ID = Factor.Id,

                };

                _Manufacture.Add(manufacture);
                //فرایند ثبت سند 
                SanadHeading sanadHeading = new SanadHeading() { Discription = string.Format("فاکتور فروش شماره " + Factor.Id + "- طرف حساب:  " + client.ClientName), FactorID = Factor.Id };
                _SanadHeading.Add(sanadHeading);
                _Sanad.Add(new Sanad()
                {
                    SanadHeading_ID = sanadHeading.Id,
                    AccountingHeading_ID = client.AccountingHeading_ID,
                    Bedehkari = Convert.ToInt32(Factor.TotalPrice),
                    Comment = string.Format("فاکتور فروش " + Factor.Id),

                });
                _Sanad.Add(new Sanad()
                {
                    SanadHeading_ID = sanadHeading.Id,
                    AccountingHeading_ID = 7,
                    Bestankari = Convert.ToInt32(Factor.TotalPrice),
                    Comment = string.Format("فاکتور فروش " + Factor.Id + client.ClientName),

                });
                if (Factor.Taxes != 0)
                {
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadHeading.Id,
                        AccountingHeading_ID = client.AccountingHeading_ID,
                        Bedehkari = Convert.ToInt32(Factor.Taxes),
                        Comment = string.Format("ارزش افزوده فاکتور شماره " + Factor.Id),

                    });
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadHeading.Id,
                        AccountingHeading_ID = 19,
                        Bestankari = Convert.ToInt32(Factor.Taxes),
                        Comment = string.Format("ارزش افزوده فاکتور شماره " + Factor.Id),

                    });
                }
                if (Factor.Discount != 0)
                {
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadHeading.Id,
                        AccountingHeading_ID = client.AccountingHeading_ID,
                        Bestankari = Convert.ToInt32(Factor.Discount),
                        Comment = string.Format("تخفیف فاکتور " + Factor.Id),

                    });
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadHeading.Id,
                        AccountingHeading_ID = 6,
                        Bedehkari = Convert.ToInt32(Factor.Discount),
                        Comment = string.Format("تخفیف فاکتور " + Factor.Id),

                    });
                }
                //ارسال اس ام اس 
                if (!string.IsNullOrEmpty(client.ClientPhone))
                {
                    try
                    {
                        SMSManager sMSManager = new SMSManager(_siteSetting.SMSConfiguration.ApiKey, _siteSetting.SMSConfiguration.SecurityCode);
                        SmsIrRestful.UltraFastParameters[] paramert =
                        {
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "UserName",
                    ParameterValue = client.ClientName
                },
                
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "FactorNumber",
                    ParameterValue = Factor.Id.ToString()
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Price",
                    ParameterValue = Factor.FactorPrice.ToString("n0")+"ريال"
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Description",
                    ParameterValue = Factor.FactorCodeView
                }
                };
                        var mobile = long.Parse(client.ClientPhone);
                        var dto = new SmsIrRestful.UltraFastSend()
                        {
                            Mobile = mobile,
                            ParameterArray = paramert,
                            TemplateId = _siteSetting.SMSConfiguration.MCFThemplateID
                        };
                        var sms = sMSManager.VerificationCodeByThemplate(dto);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return Factor.Id;
            }
            catch
            {

                return 0;
            }
        }
        public bool ChangeToFactor(int Factor_ID)
        {
            try
            {
                var factor = _Factor.GetById(Factor_ID);


                var client = _Client.GetById(factor.Client_ID);
                if (factor != null && factor.FactorType != FactorType.Factor)
                {
                    factor.FactorType = FactorType.Factor;
                    _Factor.Update(factor);
                    //ایجاد ردیف تولید
                    Manufacture manufacture = new Manufacture()
                    {
                        Factor_ID = factor.Id,

                    };

                    _Manufacture.Add(manufacture);
                    //فرایند ثبت سند 
                    SanadHeading sanadHeading = new SanadHeading() { Discription = string.Format(" فاکتور فروش شماره " + factor.Id + "- طرف حساب  " + client.ClientName), FactorID = factor.Id };
                    _SanadHeading.Add(sanadHeading);
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadHeading.Id,
                        AccountingHeading_ID = client.AccountingHeading_ID,
                        Bedehkari = Convert.ToInt32(factor.TotalPrice),
                        Comment = string.Format("فاکتور فروش " + factor.Id),

                    });
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadHeading.Id,
                        AccountingHeading_ID = 7,
                        Bestankari = Convert.ToInt32(factor.TotalPrice),
                        Comment = string.Format("فاکتور فروش " + factor.Id + client.ClientName),

                    });
                    if (factor.Taxes != 0)
                    {
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadHeading.Id,
                            AccountingHeading_ID = client.AccountingHeading_ID,
                            Bedehkari = Convert.ToInt32(factor.Taxes),
                            Comment = string.Format("ارزش افزوده فاکتور شماره " + factor.Id),

                        });
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadHeading.Id,
                            AccountingHeading_ID = 19,
                            Bestankari = Convert.ToInt32(factor.Taxes),
                            Comment = string.Format("ارزش افزوده فاکتور شماره " + factor.Id),

                        });
                    }
                    if (factor.Discount != 0)
                    {
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadHeading.Id,
                            AccountingHeading_ID = client.AccountingHeading_ID,
                            Bestankari = Convert.ToInt32(factor.Discount),
                            Comment = string.Format("تخفیف فاکتور " + factor.Id),

                        });
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadHeading.Id,
                            AccountingHeading_ID = 6,
                            Bedehkari = Convert.ToInt32(factor.Discount),
                            Comment = string.Format("تخفیف فاکتور " + factor.Id),

                        });
                    }
                    //ارسال اس ام اس 
                    if (!string.IsNullOrEmpty(client.ClientPhone))
                    {
                        try
                        {
                            SMSManager sMSManager = new SMSManager(_siteSetting.SMSConfiguration.ApiKey, _siteSetting.SMSConfiguration.SecurityCode);
                            SmsIrRestful.UltraFastParameters[] paramert =
                            {
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "UserName",
                    ParameterValue = client.ClientName
                },
                
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "FactorNumber",
                    ParameterValue = factor.Id.ToString()
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Price",
                    ParameterValue = factor.FactorPrice.ToString("n0")+"ريال"
                },
                new SmsIrRestful.UltraFastParameters()
                {
                    Parameter = "Description",
                    ParameterValue = factor.FactorCodeView
                }
                };
                            var mobile = long.Parse(client.ClientPhone);
                            var dto = new SmsIrRestful.UltraFastSend()
                            {
                                Mobile = mobile,
                                ParameterArray = paramert,
                                TemplateId = _siteSetting.SMSConfiguration.MCFThemplateID
                            };
                            var sms = sMSManager.VerificationCodeByThemplate(dto);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;

            }
        }
        public bool ChangeToPishFactor(int Factor_ID)
        {
            try
            {
                var factor = _Factor.GetById(Factor_ID);


                var client = _Client.GetById(factor.Client_ID);
                if (factor != null && factor.FactorType != FactorType.PishFactor)
                {
                    factor.FactorType = FactorType.PishFactor;

                    _Factor.Update(factor, false);
                    var sheading = _SanadHeading.Entities.Where(p => p.FactorID == Factor_ID).SingleOrDefault();
                    List<Sanad> sanads = new List<Sanad>();
                    foreach (var item in _Sanad.Entities.Where(p => p.SanadHeading_ID == sheading.Id))
                    {
                        sanads.Add(item);
                    }
                    _Sanad.DeleteRange(sanads, false);
                    _SanadHeading.Delete(sheading);
                    var manufactur = _Manufacture.Entities.Where(p => p.Factor_ID == Factor_ID).SingleOrDefault();
                    List<ManufactureHistory> manufactureHistories = new List<ManufactureHistory>();
                    foreach (var item in _ManufactureHistory.Entities.Where(p => p.Manufacture_ID == manufactur.Id))
                    {
                        manufactureHistories.Add(item);
                    }
                    _ManufactureHistory.DeleteRange(manufactureHistories, false);
                    _Manufacture.Delete(manufactur);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;

            }
        }
        public async Task<bool> UpdatePishFactor(int FactorID, AddFactorViewModel AddFactorViewModel)
        {
            try
            {
                var factor = _Factor.GetById(FactorID);
                if (factor != null)
                {
                    List<Product_Factor> listp = new List<Product_Factor>();
                    foreach (var item in _Product_Factor.Entities.Where(p => p.Factor_ID == FactorID))
                    {
                        listp.Add(item);
                    }
                    _Product_Factor.DeleteRange(listp, false);


                    factor.User_ID = AddFactorViewModel.User_ID;
                    factor.Discription = AddFactorViewModel.Discription;
                    //factor.FactorType = factor.FactorType;
                    var client = _Client.GetById(AddFactorViewModel.Client_ID);
                    factor.Client_ID = client.Id;
                    factor.TotalPrice = 0;
                    if (AddFactorViewModel.rows != null)
                    {
                        foreach (var item in AddFactorViewModel.rows)
                        {
                            var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                            item.UnitPrice = p.UnitPrice;
                            if (p.UnitType == UnitType.SquareMeters)
                                factor.TotalPrice += item.Width * item.length * item.Unit * (decimal)item.UnitPrice;
                            if (p.UnitType == UnitType.Metr)
                                factor.TotalPrice += item.length * item.Unit * (decimal)item.UnitPrice;
                            if (p.UnitType == UnitType.Unit)
                                factor.TotalPrice += item.Unit * (decimal)item.UnitPrice;

                        }
                    }

                    factor.Discount = factor.TotalPrice * (client.DiscountPercent / 100);
                    if (!AddFactorViewModel.discountDefoult)
                        factor.Discount = AddFactorViewModel.discoun;
                    factor.FactorPrice = factor.TotalPrice - factor.Discount;
                    factor.Taxes = 0;
                    if (AddFactorViewModel.Rasmi)
                        factor.Taxes = factor.FactorPrice * Convert.ToDecimal(0.09);
                    factor.FactorPrice = factor.FactorPrice + factor.Taxes;

                    _Factor.Update(factor, false);
                    //ذخیره ردیف های فاکتور

                    List<Product_Factor> listrow = new List<Product_Factor>();
                    foreach (var item in AddFactorViewModel.rows)
                    {
                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        var r = new Product_Factor();

                        r.Factor_ID = factor.Id;
                        r.length = item.length;
                        r.Width = item.Width;
                        r.Unit = item.Unit;
                        r.UnitPrice = item.UnitPrice;
                        r.ProductAndService_ID = item.ProductAndService_ID;
                        if (p.UnitType == UnitType.SquareMeters)
                            r.Price = (double)(item.Width * item.length * item.Unit * (decimal)item.UnitPrice);
                        if (p.UnitType == UnitType.Metr)
                            r.Price = (double)(item.length * item.Unit * (decimal)item.UnitPrice);
                        if (p.UnitType == UnitType.Unit)
                            r.Price = item.Unit * item.UnitPrice;
                        r.RowDiscription = item.RowDiscription;
                        listrow.Add(r);
                    }
                    _Product_Factor.AddRange(listrow);



                }


                return true;
            }
            catch
            {

                return false;
            }
        }
        public async Task<bool> UpdatePartnerFactor(int FactorID, AddFactorPartnerViewModel AddFactorViewModel)
        {
            try
            {
                var factor = _Factor.GetById(FactorID);
                if (factor != null)
                {
                    List<Product_Factor> listp = new List<Product_Factor>();
                    foreach (var item in _Product_Factor.Entities.Where(p => p.Factor_ID == FactorID))
                    {
                        listp.Add(item);
                    }
                    _Product_Factor.DeleteRange(listp, false);


                    factor.User_ID = AddFactorViewModel.UserID;
                    factor.Discription = AddFactorViewModel.Discription;
                    //factor.FactorType = factor.FactorType;
                    var client = _Client.TableNoTracking.Where(p => p.User_ID == AddFactorViewModel.UserID).FirstOrDefault();
                    factor.Client_ID = client.Id;
                    factor.TotalPrice = 0;
                    if (AddFactorViewModel.rows != null)
                    {
                        foreach (var item in AddFactorViewModel.rows)
                        {
                            var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                            item.UnitPrice = p.UnitPrice;
                            if (p.UnitType == UnitType.SquareMeters)
                                factor.TotalPrice += item.Width * item.length * item.Unit * (decimal)item.UnitPrice;
                            if (p.UnitType == UnitType.Metr)
                                factor.TotalPrice += item.length * item.Unit * (decimal)item.UnitPrice;
                            if (p.UnitType == UnitType.Unit)
                                factor.TotalPrice += item.Unit * (decimal)item.UnitPrice;

                        }
                    }

                    factor.Discount = factor.TotalPrice * (client.DiscountPercent / 100);

                    factor.FactorPrice = factor.TotalPrice - factor.Discount;
                    factor.Taxes = 0;

                    factor.FactorPrice = factor.FactorPrice + factor.Taxes;

                    _Factor.Update(factor, false);
                    //ذخیره ردیف های فاکتور

                    List<Product_Factor> listrow = new List<Product_Factor>();
                    foreach (var item in AddFactorViewModel.rows)
                    {
                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        var r = new Product_Factor();

                        r.Factor_ID = factor.Id;
                        r.length = item.length;
                        r.Width = item.Width;
                        r.Unit = item.Unit;
                        r.UnitPrice = item.UnitPrice;
                        r.ProductAndService_ID = item.ProductAndService_ID;
                        if (p.UnitType == UnitType.SquareMeters)
                            r.Price = (double)(item.Width * item.length * item.Unit * (decimal)item.UnitPrice);
                        if (p.UnitType == UnitType.Metr)
                            r.Price = (double)(item.length * item.Unit * (decimal)item.UnitPrice);
                        if (p.UnitType == UnitType.Unit)
                            r.Price = item.Unit * item.UnitPrice;
                        r.RowDiscription = item.RowDiscription;
                        listrow.Add(r);
                    }
                    _Product_Factor.AddRange(listrow);



                }


                return true;
            }
            catch
            {

                return false;
            }
        }
        public async Task<bool> UpdateFactor(int FactorID, AddFactorViewModel AddFactorViewModel)
        {
            try
            {
                var factor = _Factor.GetById(FactorID);
                if (factor != null)
                {
                    List<Product_Factor> list = new List<Product_Factor>();
                    foreach (var item in _Product_Factor.Entities.Where(p => p.Factor_ID == FactorID))
                    {
                        list.Add(item);
                    }
                    _Product_Factor.DeleteRange(list, false);


                    factor.User_ID = AddFactorViewModel.User_ID;
                    //factor.FactorType = factor.FactorType;
                    var client = _Client.GetById(AddFactorViewModel.Client_ID);
                    factor.Client_ID = client.Id;
                    factor.TotalPrice = 0;
                    if (AddFactorViewModel.rows != null)
                    {
                        foreach (var item in AddFactorViewModel.rows)
                        {
                            var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                            item.UnitPrice = p.UnitPrice;
                            if (p.UnitType == UnitType.SquareMeters)
                                factor.TotalPrice += item.Width * item.length * item.Unit * (decimal)item.UnitPrice;
                            if (p.UnitType == UnitType.Metr)
                                factor.TotalPrice += item.length * item.Unit * (decimal)item.UnitPrice;
                            if (p.UnitType == UnitType.Unit)
                                factor.TotalPrice += item.Unit * (decimal)item.UnitPrice;

                        }
                    }
                    factor.Discription = AddFactorViewModel.Discription;
                    factor.Discount = factor.TotalPrice * (client.DiscountPercent / 100);
                    if (!AddFactorViewModel.discountDefoult)
                        factor.Discount = AddFactorViewModel.discoun;
                    factor.FactorPrice = factor.TotalPrice - factor.Discount;
                    factor.Taxes = 0;
                    if (AddFactorViewModel.Rasmi)
                        factor.Taxes = factor.FactorPrice * Convert.ToDecimal(0.09);
                    factor.FactorPrice = factor.FactorPrice + factor.Taxes;

                    _Factor.Update(factor, false);
                    //ذخیره ردیف های فاکتور

                    List<Product_Factor> listrow = new List<Product_Factor>();
                    foreach (var item in AddFactorViewModel.rows)
                    {
                        var p = _ProductAndService.Entities.Find(item.ProductAndService_ID);
                        var r = new Product_Factor();

                        r.Factor_ID = factor.Id;
                        r.length = item.length;
                        r.Width = item.Width;
                        r.Unit = item.Unit;
                        r.UnitPrice = item.UnitPrice;
                        r.ProductAndService_ID = item.ProductAndService_ID;
                        if (p.UnitType == UnitType.SquareMeters)
                            r.Price = (double)item.Width * (double)item.length * item.Unit * item.UnitPrice;
                        if (p.UnitType == UnitType.Metr)
                            r.Price = (double)item.length * item.Unit * item.UnitPrice;
                        if (p.UnitType == UnitType.Unit)
                            r.Price = item.Unit * item.UnitPrice;
                        r.RowDiscription = item.RowDiscription;
                        listrow.Add(r);
                    }
                    _Product_Factor.AddRange(listrow, false);
                    var sanadheading = _SanadHeading.Entities.Where(prop => prop.FactorID == factor.Id).SingleOrDefault();

                    List<Sanad> sanadlist = new List<Sanad>();
                    foreach (var item in _Sanad.Entities.Where(p => p.SanadHeading_ID == sanadheading.Id))
                    {
                        sanadlist.Add(item);
                    }
                    _Sanad.DeleteRange(sanadlist, false);
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadheading.Id,
                        AccountingHeading_ID = client.AccountingHeading_ID,
                        Bedehkari = Convert.ToInt32(factor.TotalPrice),
                        Comment = string.Format("فاکتور فروش " + factor.Id),

                    }, false);
                    _Sanad.Add(new Sanad()
                    {
                        SanadHeading_ID = sanadheading.Id,
                        AccountingHeading_ID = 7,
                        Bestankari = Convert.ToInt32(factor.TotalPrice),
                        Comment = string.Format("فاکتور فروش " + factor.Id + client.ClientName),

                    });
                    if (factor.Taxes != 0)
                    {
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadheading.Id,
                            AccountingHeading_ID = client.AccountingHeading_ID,
                            Bedehkari = Convert.ToInt32(factor.Taxes),
                            Comment = string.Format("مبلغ ارزش افزوده فاکتور فروش " + factor.Id),

                        }, false);
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadheading.Id,
                            AccountingHeading_ID = 19,
                            Bestankari = Convert.ToInt32(factor.Taxes),
                            Comment = string.Format("مبلغ ارزش افزوده فاکتور فروش " + factor.Id),

                        });
                    }
                    if (factor.Discount != 0)
                    {
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadheading.Id,
                            AccountingHeading_ID = client.AccountingHeading_ID,
                            Bestankari = Convert.ToInt32(factor.Discount),
                            Comment = string.Format("تخفیف فاکتور " + factor.Id),

                        }, false);
                        _Sanad.Add(new Sanad()
                        {
                            SanadHeading_ID = sanadheading.Id,
                            AccountingHeading_ID = 6,
                            Bedehkari = Convert.ToInt32(factor.Discount),
                            Comment = string.Format("تخفیف فاکتور " + factor.Id),

                        });
                    }

                }


                return true;
            }
            catch
            {

                return false;
            }
        }
        public async Task<bool> DeleteFactor(int FactorID)
        {
            try
            {
                var factor = _Factor.GetById(FactorID);
                List<Product_Factor> l = new List<Product_Factor>();
                foreach (var item in _Product_Factor.Entities.Where(p => p.Factor_ID == FactorID))
                {
                    l.Add(item);
                }

                _Product_Factor.DeleteRange(l, false);

                List<FactorAttachment> lattach = new List<FactorAttachment>();
                foreach (var item in _FactorAttachment.Entities.Where(p => p.Facor_ID == FactorID))
                {
                    lattach.Add(item);
                }
                _FactorAttachment.DeleteRange(lattach, false);
                var manufactur = _Manufacture.Entities.Where(p => p.Factor_ID == FactorID).SingleOrDefault();
                List<ManufactureHistory> manufactureHistories = new List<ManufactureHistory>();
                if (manufactur != null)
                {
                    var list = _ManufactureHistory.Entities.Where(p => p.Manufacture_ID == manufactur.Id).ToList();
                    foreach (var item in list)
                    {
                        manufactureHistories.Add(item);
                    }
                    _ManufactureHistory.DeleteRange(manufactureHistories, false);
                    _Manufacture.Delete(manufactur);
                }

                var sanadheading = _SanadHeading.Entities.Where(p => p.FactorID == FactorID).SingleOrDefault();
                List<Sanad> sanad = new List<Sanad>();
                if (sanadheading != null)
                {
                    foreach (var item in _Sanad.Entities.Where(p => p.SanadHeading_ID == sanadheading.Id).ToList())
                    {
                        sanad.Add(item);
                    }
                    _Sanad.DeleteRange(sanad, false);
                    _SanadHeading.Delete(sanadheading, false);
                }
                var experthistory = _ExpertHistory.Entities.Where(p => p.Facor_ID == factor.Id).SingleOrDefault();
                if (experthistory != null)
                {
                    _ExpertHistory.Delete(experthistory);
                }

                _Factor.Delete(factor);

                return true;
            }
            catch
            {

                return false;
            }
        }
        public async Task<bool> AddFactorAttachment(int FactorID, IFormFile file)
        {
            try
            {
                if (file != null && file.IsImage())
                {
                    bool exists = System.IO.Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + FactorID));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + FactorID));
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + FactorID);
                    int CountFile = System.IO.Directory.GetFiles(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + FactorID)).Length;
                    string FName = FactorID.ToString() + CountFile.ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploads, FName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    _FactorAttachment.Add(new FactorAttachment()
                    {
                        Facor_ID = FactorID,
                        FileName = FName,

                    });
                    return true;
                }

                return false;
            }
            catch
            {

                return false;
            }
        }

        #endregion

        #region Accounting
        public async Task<bool> AddToBank(Bank bank)
        {
            try
            {
                var acc = new AccountingHeading()
                {
                    Title = bank.BankTitle,
                    AccountingType = AccountingType.Crediting,
                    Discription = "حساب بانکی ",
                };
                _AccountingHeading.Add(acc, false);
                bank.AccountingHeading_ID = acc.Id;

                _Bank.Add(bank);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public async Task<bool> UpdateBank(int id, Bank bank)
        {
            try
            {
                var ban = _Bank.GetById(id);
                if (ban != null)
                {
                    ban.BankTitle = bank.BankTitle;
                    _Bank.Update(ban);
                    var a = _AccountingHeading.GetById(ban.AccountingHeading_ID);
                    a.Title = ban.BankTitle;
                    _AccountingHeading.Update(a);
                }

                return true;
            }
            catch
            {

                return false;
            }
        }
        public async Task<int> addSanad(sanadViewModel sanadViewModel)
        {
            try
            {
                SanadHeading sh = new SanadHeading();
                sh.Discription = sanadViewModel.Discription;
                _SanadHeading.Add(sh);
                List<Sanad> list = new List<Sanad>();
                foreach (var item in sanadViewModel.Sanads)
                {
                    Sanad s = new Sanad()
                    {
                        AccountingHeading_ID = item.AccountingHeading_ID,
                        Bedehkari = item.Bedehkari,
                        Bestankari = item.Bestankari,
                        Comment = item.Comment,
                        SanadHeading_ID = sh.Id

                    };
                    list.Add(s);
                }
                _Sanad.AddRange(list);

                return sh.Id;
            }
            catch
            {

                return 0;
            }
        }
        public async Task<int> UpdateSanad(int SanadID, sanadViewModel sanadViewModel)
        {
            try
            {
                SanadHeading sh = _SanadHeading.GetById(SanadID);
                sh.Discription = sanadViewModel.Discription;
                _SanadHeading.Update(sh, false);
                List<Sanad> listsanad = new List<Sanad>();
                foreach (var item in _Sanad.Entities.Where(p => p.SanadHeading_ID == SanadID))
                {
                    listsanad.Add(item);
                }
                _Sanad.DeleteRange(listsanad, false);
                List<Sanad> list = new List<Sanad>();

                foreach (var item in sanadViewModel.Sanads)
                {
                    Sanad s = new Sanad()
                    {
                        AccountingHeading_ID = item.AccountingHeading_ID,
                        Bedehkari = item.Bedehkari,
                        Bestankari = item.Bestankari,
                        Comment = item.Comment,
                        SanadHeading_ID = sh.Id

                    };
                    list.Add(s);
                }
                _Sanad.AddRange(list);

                return sh.Id;
            }
            catch
            {

                return 0;
            }
        }
        public async Task<bool> DeletSanad(int SanadHeading)
        {
            try
            {
                var sanadh = _SanadHeading.GetById(SanadHeading);
                List<Sanad> sanads = new List<Sanad>();
                foreach (var item in _Sanad.Entities.Where(p => p.SanadHeading_ID == sanadh.Id))
                {
                    sanads.Add(item);
                }
                _Sanad.DeleteRange(sanads, false);
                _SanadHeading.Delete(sanadh);
                return true;
            }
            catch
            {

                return false;
            }
        }
        public async Task<ClientAccountingStatus> ClientAccountingStatus(int ClientID)
        {
            int a = _Client.TableNoTracking.SingleOrDefault(p => p.Id == ClientID).AccountingHeading_ID;
            try
            {
                if (a != 0 && a != null)
                {
                    Int64 bedehkari = _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == a && p.Bedehkari > 0).Sum(p => p.Bedehkari);
                    Int64 Bestankari = _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == a && p.Bestankari > 0).Sum(p => p.Bestankari);
                    var result = Bestankari - bedehkari;
                    ClientAccountingStatus respons = new ClientAccountingStatus();

                    if (result > 0)
                        respons.status = "بستانکار";
                    if (result < 0)
                        respons.status = "بدهکار";
                    if (result == 0)
                        respons.status = "---";
                    respons.Price = result;
                    return respons;

                }
                return null;
            }
            catch (Exception)
            {

                return null;
            }


        }
        public async Task<bool> AddsanadAttachment(int sanadid, IFormFile file)
        {
            try
            {
                if (file != null && file.IsImage())
                {
                    bool exists = System.IO.Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Sanads/" + sanadid));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Sanads/" + sanadid));
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Sanads/" + sanadid);
                    int CountFile = System.IO.Directory.GetFiles(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Sanads/" + sanadid)).Length;
                    string FName = sanadid.ToString() + CountFile.ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploads, FName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    _SanadAttachment.Add(new SanadAttachment()
                    {
                        SanadID = sanadid,
                        FileName = FName,

                    });
                    return true;
                }

                return false;
            }
            catch
            {

                return false;
            }
        }


        #endregion

    }
}
