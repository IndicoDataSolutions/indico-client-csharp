﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;

namespace IndicoV2.DataSets
{
    public interface IDataSetClient
    {
        /// <summary>
        /// Lists all <seealso cref="IDataSet"/>s
        /// </summary>
        Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken = default);
    }
}
