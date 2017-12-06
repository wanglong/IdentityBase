// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityBase.WebApi.Actions.Invitations
{
    using System;
    using System.Threading.Tasks;
    using IdentityBase.Models;
    using IdentityBase.Services;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Mvc;
    using ServiceBase.Authorization;

    public class InvitationsDeleteController : WebApiController
    {
        private readonly UserAccountService _userAccountService;

        public InvitationsDeleteController(
            UserAccountService userAccountService)
        {
            this._userAccountService = userAccountService;
        }
        
        [HttpDelete("invitations/{UserAccountId}")]
        [ScopeAuthorize(WebApiConstants.ApiName, AuthenticationSchemes =
            IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete([FromRoute]Guid userAccountId)
        {
            UserAccount userAccount = await this._userAccountService
                .LoadByIdAsync(userAccountId);

            if (userAccount == null ||
                userAccount.CreationKind != CreationKind.Invitation)
            {
                return this.NotFound();
            }

            if (userAccount.IsEmailVerified)
            {
                return this.BadRequest(
                    "Invitation is already confirmed and cannot be deleted");
            }

            await this._userAccountService.DeleteByIdAsync(userAccountId);

            return this.Ok(); 
        }
    }
}