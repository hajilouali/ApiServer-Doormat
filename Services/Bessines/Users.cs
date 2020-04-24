using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Services.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Bessines
{
    public class UsersProcess
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<Client> _clientrepository;
        private readonly IRepository<AccountingHeading> _accountingHeadingrepository;
        private readonly IRepository<Factor> _Factor;
        private readonly IRepository<Expert> _Expert;
        public UsersProcess()
        {
        }

        public UsersProcess(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IRepository<Client> Clientrepository,
            IRepository<AccountingHeading> AccountingHeadingrepository,
            IRepository<Factor> factor,
            IRepository<Expert> expert)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _clientrepository = Clientrepository;
            _accountingHeadingrepository = AccountingHeadingrepository;
            _Factor = factor;
            _Expert = expert;
        }

        public async Task<bool> AddUserToApp(User user, List<string> Rolles)
        {
            if (user.UserName != null)
            {
                await _userManager.CreateAsync(user, user.PasswordHash);
                if (Rolles != null)
                {

                    await _userManager.AddToRolesAsync(user, Rolles);

                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "CellPartner");
                }
                AccountingHeading accountingHeading = new AccountingHeading()
                {
                    Title = user.FullName,
                    AccountingType = AccountingType.debt,
                    Discription = "طرف حساب"
                };
                _accountingHeadingrepository.Add(accountingHeading, true);
                Client client = new Client()
                {
                    ClientName = user.FullName,
                    User_ID = user.Id,
                    AccountingHeading_ID = accountingHeading.Id,

                };
                _clientrepository.Add(client, true);



                return true;
            }


            return false;
        }
        public async Task<bool> addClient(Client client)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(client.User_ID.ToString());
                if (user != null)
                {
                    var h = new AccountingHeading()
                    {
                        AccountingType = AccountingType.debt,
                        Title = client.ClientName,
                        Discription = "طرف حساب",

                    };
                    _accountingHeadingrepository.Add(h);
                    client.AccountingHeading_ID = h.Id;
                    _clientrepository.Add(client);

                    return true;
                }
                return false;
            }
            catch
            {

                return false;
            }
        }
        public async Task<bool> UpdateClient(int ClientID, Client client)
        {
            try
            {
                var C = await _clientrepository.Entities.FindAsync(ClientID);
                if (C != null)
                {
                    var acc = _accountingHeadingrepository.GetById(C.AccountingHeading_ID);
                    acc.Title = client.ClientName;
                    _accountingHeadingrepository.Update(acc);
                    C.ClientName = client.ClientName;
                    C.ClientAddress = client.ClientAddress;
                    C.ClientPhone = client.ClientPhone;
                    C.ClientPartnerName = client.ClientPartnerName;
                    C.CodeEgtesadi = client.CodeEgtesadi;
                    C.CodeMeli = client.CodeMeli;
                    C.DiscountPercent = client.DiscountPercent;
                    C.MaxCreditValue = client.MaxCreditValue;
                    
                    _clientrepository.Update(C);
                    return true;
                }
                return false;
            }
            catch
            {

                return false;
            }
        }
        public async Task<bool> DeletClient(int ClientID)
        {
            try
            {
                var Client = _clientrepository.GetById(ClientID);
                if (Client != null)
                {
                    var result = _accountingHeadingrepository.GetById(Client.AccountingHeading_ID);

                    if (result.Sanads==null )
                    {
                        if (_Factor.Entities.Where(p => p.Client_ID == ClientID).Any()==false)
                        {
                            if (_Expert.Entities.Where(p => p.Client_ID == ClientID).Any() == false)
                            {
                                _clientrepository.Delete(Client);
                                _accountingHeadingrepository.Delete(result);
                                
                                return true;
                            }
                        }
                        
                    }
                    
                }
                return false;
            }
            catch
            {

                return false;
            }
        }
    }
}
