using Datos;
using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vista
{
    public partial class TicketForm : Form
    {
        public TicketForm()
        {
            InitializeComponent();
        }

        Cliente miCliente = null;
        ClienteDB clienteDB = new ClienteDB();
        List<DetalleTicket> listaDetalles = new List<DetalleTicket>();
        TicketDB ticketDB = new TicketDB();

        decimal subTotal = 0;
        decimal isv = 0;
        decimal descuento = 0;
        decimal totalAPagar = 0;

        private void LimpiarControles()
        {
            miCliente = null;
            listaDetalles = null;
            FechaDateTimePicker.Value = DateTime.Now;
            IdentidadTextBox.Clear();
            NombreClienteTextBox.Clear();
            TipoSoporteComboBox = null;
            //DescripcionSolicitudTextBox.Clear();
            //DescripcionRespuestaTextBox.Clear();
            PrecioTextBox.Clear();
            DetalleDataGridView.DataSource = null;
            subTotal = 0;
            //SubtotalTextBox.Clear();
            isv = 0;
            ISVTextBox.Clear();
            descuento = 0;
            DescuentoTextBox.Clear();
            totalAPagar = 0;
            //TotalAPagarTextBox.Clear();
        }

        private void IdentidadTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(IdentidadTextBox.Text))
            {
                miCliente = new Cliente();
                miCliente = clienteDB.DevolverClientePorIdentidad(IdentidadTextBox.Text);
                NombreClienteTextBox.Text = miCliente.Nombre;
            }
            else
            {
                miCliente = null;
                NombreClienteTextBox.Clear();
            }
        }

        private void TicketForm_Load(object sender, EventArgs e)
        {
            UsuarioTextBox.Text = System.Threading.Thread.CurrentPrincipal.Identity.Name;
        }

        private void BuscarClienteButton_Click(object sender, EventArgs e)
        {
            BuscarClienteForm buscar = new BuscarClienteForm();
            buscar.ShowDialog();

            miCliente = new Cliente();
            miCliente = buscar.cliente;
            IdentidadTextBox.Text = miCliente.Identidad;
            NombreClienteTextBox.Text = miCliente.Nombre;
        }

        private void PrecioTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }


            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }



            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!string.IsNullOrEmpty(PrecioTextBox.Text) && !string.IsNullOrEmpty(TipoSoporteComboBox.Text) && !string.IsNullOrEmpty(SolicitudTextBox.Text) && !string.IsNullOrEmpty(RespuestaTextBox.Text))
                {

                    DetalleTicket detalle = new DetalleTicket();
                    detalle.TipoSoporte = TipoSoporteComboBox.Text;
                    detalle.DescripcionSolicitud = SolicitudTextBox.Text;
                    detalle.DescripcionRespuesta = RespuestaTextBox.Text;
                    detalle.Precio = Convert.ToDecimal(PrecioTextBox.Text);
                    detalle.Total = detalle.Precio;


                    subTotal += detalle.Total;
                    isv = subTotal * 0.15M;
                    totalAPagar = subTotal + isv;

                    listaDetalles.Add(detalle);
                    DetalleDataGridView.DataSource = null;
                    DetalleDataGridView.DataSource = listaDetalles;


                    SubTotalTextBox.Text = subTotal.ToString("N2");
                    ISVTextBox.Text = isv.ToString("N2");
                    TotalTextBox.Text = totalAPagar.ToString("N2");

                    TipoSoporteComboBox.Text = "";
                    SolicitudTextBox.Clear();
                    RespuestaTextBox.Clear();
                    PrecioTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Ingresar los Datos del Detalle Completo", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
        }

        private void GuardarButton_Click(object sender, EventArgs e)
        {

            Ticket miticket = new Ticket();
            miticket.Fecha = FechaDateTimePicker.Value;
            miticket.IdentidadCliente = miCliente.Identidad;
            miticket.CodigoUsuario = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            miticket.SubTotal = subTotal;
            miticket.ISV = isv;
            miticket.Descuento = descuento;
            miticket.Total = totalAPagar;

            bool inserto = ticketDB.Guardar(miticket, listaDetalles);
            if (inserto)
            {
                IdentidadTextBox.Focus();
                MessageBox.Show("Ticket se Guardo Exitosamente", "Excelente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarControles();
            }
            else
            {
                MessageBox.Show("No se Pudo Guardar el Ticket", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelarButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DescuentoTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(DescuentoTextBox.Text))
            {
                descuento = Convert.ToDecimal(DescuentoTextBox.Text);
                totalAPagar = totalAPagar - descuento;
                TotalTextBox.Text = totalAPagar.ToString();
            }
        }

        private void AgregarButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PrecioTextBox.Text) && !string.IsNullOrEmpty(TipoSoporteComboBox.Text) && !string.IsNullOrEmpty(SolicitudTextBox.Text) && !string.IsNullOrEmpty(RespuestaTextBox.Text))
            {

                DetalleTicket detalle = new DetalleTicket(); 
                detalle.TipoSoporte = TipoSoporteComboBox.Text;
                detalle.DescripcionSolicitud = SolicitudTextBox.Text;
                detalle.DescripcionRespuesta = RespuestaTextBox.Text;
                detalle.Precio = Convert.ToDecimal(PrecioTextBox.Text);
                detalle.Total = detalle.Precio;

                subTotal += detalle.Total;
                isv = subTotal * 0.15M; 
                totalAPagar = subTotal + isv;

                listaDetalles.Add(detalle); 

                DetalleDataGridView.DataSource = null;
                DetalleDataGridView.DataSource = listaDetalles;

                SubTotalTextBox.Text = subTotal.ToString("N2");
                ISVTextBox.Text = isv.ToString("N2");
                TotalTextBox.Text = totalAPagar.ToString("N2");

                TipoSoporteComboBox.Text = "";
                SolicitudTextBox.Clear();
                RespuestaTextBox.Clear();
                PrecioTextBox.Clear();

            }
            else
            {
                MessageBox.Show("Debe Ingresar todos los Datos de Detalle", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}    


