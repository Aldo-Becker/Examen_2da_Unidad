using Entidades;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class TicketDB
    {
        string cadena = "server=localhost; user=root; database=ticket; password=123456";

        public bool Guardar(Ticket ticket)
        {
            bool inserto = false;
            int idTicket = 0;
            try
            {
                StringBuilder sqlFactura = new StringBuilder();
                sqlFactura.Append(" INSERT INTO factura (Fecha, IdentidadCliente, CodigoUsuario, ISV, Descuento, SubTotal, Total) VALUES (@Fecha, @IdentidadCliente, @CodigoUsuario, @ISV, @Descuento, @SubTotal, @Total); ");
                sqlFactura.Append(" SELECT LAST_INSERT_ID(); ");

                StringBuilder sqlDetalle = new StringBuilder();
                sqlDetalle.Append(" INSERT INTO detallefactura (IdFactura, CodigoProducto, Precio, Cantidad, Total) VALUES (@IdFactura, @CodigoProducto, @Precio, @Cantidad, @Total); ");

                StringBuilder sqlExistencia = new StringBuilder();
                sqlExistencia.Append(" UPDATE producto SET Existencia = Existencia - @Cantidad WHERE Codigo = @Codigo; ");

                using (MySqlConnection con = new MySqlConnection(cadena))
                {
                    con.Open();

                    MySqlTransaction transaction = con.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                    try
                    {
                        using (MySqlCommand cmd1 = new MySqlCommand(sqlFactura.ToString(), con, transaction))
                        {
                            cmd1.CommandType = System.Data.CommandType.Text;
                            cmd1.Parameters.Add("@Fecha", MySqlDbType.DateTime).Value = ticket.Fecha;
                            cmd1.Parameters.Add("@IdentidadCliente", MySqlDbType.VarChar, 25).Value = ticket.IdentidadCliente;
                            cmd1.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = ticket.CodigoUsuario;
                            cmd1.Parameters.Add("@TipoSoporte", MySqlDbType.VarChar, 50).Value = ticket.TipoSoporte;
                            cmd1.Parameters.Add("@DescripcionSolicitud", MySqlDbType.VarChar, 100).Value = ticket.DescripcionSolicitud;
                            cmd1.Parameters.Add("@DescripcionRespuesta", MySqlDbType.VarChar, 100).Value = ticket.DescripcionRespuesta;
                            cmd1.Parameters.Add("@Precio", MySqlDbType.Decimal).Value = ticket.Precio;
                            cmd1.Parameters.Add("@ISV", MySqlDbType.Decimal).Value = ticket.ISV;
                            cmd1.Parameters.Add("@Descuento", MySqlDbType.Decimal).Value = ticket.Descuento;
                            cmd1.Parameters.Add("@Total", MySqlDbType.Decimal).Value = ticket.Total;
                            idTicket = Convert.ToInt32(cmd1.ExecuteScalar());
                        }

                      
                        transaction.Commit();
                        inserto = true;
                    }
                    catch (System.Exception)
                    {
                        inserto = false;
                        transaction.Rollback();
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return inserto;
        }

    }
}
