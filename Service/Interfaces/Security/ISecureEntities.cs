using Sabio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type used for UserId</typeparam>
    /// <typeparam name="K">Type used for EntityId</typeparam>
    public interface ISecureEntities<T, K>
    {
        bool IsAuthorized(T userId, K entityId, EntityActionType actionType, EntityType entityType);
    }
}
