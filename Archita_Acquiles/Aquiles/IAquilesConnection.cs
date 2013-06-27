using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles
{
    /// <summary>
    /// Interface for any AquilesConnection with minimum methods to operate with.
    /// </summary>
    public interface IAquilesConnection : IDisposable
    {
        /// <summary>
        /// Retrieves an opened connection from the cluster's pool. 
        /// <remarks>When calling this method explicity, automatic command retry is not posible. Using this methods delegates the close responsability to the client using the conection.</remarks>
        /// </summary>
        void Open();

        /// <summary>
        /// Release or invalidate (depends on the status of the connection) the opened connection to the cluster's pool.
        /// <remarks>When calling tihs method explicity, a previous open method must be called from client code.</remarks>
        /// </summary>
        void Close();

        /// <summary>
        /// Execute the <see cref="Aquiles.IAquilesCommand"/> over the connection.
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// <remarks>It validates the command before proceeding, so it might throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> if there are malformed or missing input params.</remarks>
        /// </summary>
        /// <param name="command">command to be executed. Must implement IAquilesCommand</param>
        void Execute(IAquilesCommand command);
    }
}
