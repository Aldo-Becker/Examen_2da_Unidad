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

        public bool Guardar(Ticket ticket, List<DetalleTicket> detalleTickets)
        {
            bool inserto = false;
            int idTicket = 0;

            try
            {
    
                StringBuilder sqlTicket = new StringBuilder();
                sqlTicket.Append("INSERT INTO ticket (Fecha, IdentidadCliente, CodigoUsuario, SubTotal, ISV, Descuento, Total) VALUES (@Fecha, @IdentidadCliente, @CodigoUsuario, @SubTotal, @ISV, @Descuento, @Total);");
                sqlTicket.Append(" SELECT LAST_INSERT_ID();"); 
               
                StringBuilder sqlDetalle = new StringBuilder();
                sqlDetalle.Append(" INSERT INTO detalleticket (IdTicket, TipoSoporte, DescripcionSolicitud, DescripcionRespuesta, Precio, Total) VALUES (@IdTicket, @TipoSoporte, @DescripcionSolicitud, @DescripcionRespuesta, @Precio, @Total);");

                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open(); 

              
                    MySqlTransaction transaction = _conexion.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                    try
                    {
                        using (MySqlCommand cmd1 = new MySqlCommand(sqlTicket.ToString(), _conexion, transaction))
                        {
                            cmd1.CommandType = System.Data.CommandType.Text;

                            cmd1.Parameters.Add("@Fecha", MySqlDbType.DateTime).Value = ticket.Fecha;
                            cmd1.Parameters.Add("@IdentidadCliente", MySqlDbType.VarChar, 25).Value = ticket.IdentidadCliente;
                            cmd1.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = ticket.CodigoUsuario;
                            cmd1.Parameters.Add("@SubTotal", MySqlDbType.Decimal).Value = ticket.SubTotal;
                            cmd1.Parameters.Add("@ISV", MySqlDbType.Decimal).Value = ticket.ISV;
                            cmd1.Parameters.Add("@Descuento", MySqlDbType.Decimal).Value = ticket.Descuento;
                            cmd1.Parameters.Add("@Total", MySqlDbType.Decimal).Value = ticket.Total;
                            idTicket = Convert.ToInt32(cmd1.ExecuteScalar()); 

                        }

         
                        foreach (DetalleTicket detalle in detalleTickets)
                        {

                            using (MySqlCommand cmd2 = new MySqlCommand(sqlDetalle.ToString(), _conexion, transaction))
                            {
                                cmd2.CommandType = System.Data.CommandType.Text;

                                cmd2.Parameters.Add("@IdTicket", MySqlDbType.Int32).Value = idTicket;
                                cmd2.Parameters.Add("@TipoSoporte", MySqlDbType.VarChar, 45).Value = detalle.TipoSoporte;
                                cmd2.Parameters.Add("@DescripcionSolicitud", MySqlDbType.VarChar, 100).Value = detalle.DescripcionSolicitud;
                                cmd2.Parameters.Add("@DescripcionRespuesta", MySqlDbType.VarChar, 100).Value = detalle.DescripcionRespuesta;
                                cmd2.Parameters.Add("@Precio", MySqlDbType.Decimal).Value = detalle.Precio;
                                cmd2.Parameters.Add("@Total", MySqlDbType.Decimal).Value = detalle.Total;
                                cmd2.ExecuteNonQuery(); 

                            }
                        }

                        transaction.Commit(); 
                        inserto = true; 

                    }
                    catch (System.Exception ex)
                    {
                        inserto = false; 
                        transaction.Rollback(); 
                        throw;
                    }

                }

            }

            catch (System.Exception ex)
            {

            }
            return inserto;

        }
    }
}
