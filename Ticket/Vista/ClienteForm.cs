using Datos;
using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vista
{
    public partial class ClienteForm : Form
    {
        public ClienteForm()
        {
            InitializeComponent();
        }
        string operacion; 
        Cliente cliente;
        ClienteDB clientesDB = new ClienteDB();

        private void HabilitarControles()
        {
            IdentidadTextBox.Enabled = true;
            NombreTextBox.Enabled = true;
            TelefonoTextBox.Enabled = true;
            CorreoTextBox.Enabled = true;
            DireccionTextBox.Enabled = true;
            FechaDateTimePicker.Enabled = true;
            EstaActivoCheckBox.Enabled = true;
            GuardarButton.Enabled = true;
            CancelarButton.Enabled = true;
            NuevoButton.Enabled = false;
        }

        private void DeshabilitarControles()
        {
            IdentidadTextBox.Enabled = false;
            NombreTextBox.Enabled = false;
            TelefonoTextBox.Enabled = false;
            CorreoTextBox.Enabled = false;
            DireccionTextBox.Enabled = false;
            FechaDateTimePicker.Enabled = false;
            EstaActivoCheckBox.Enabled = false;
            GuardarButton.Enabled = false;
            CancelarButton.Enabled = false;
            NuevoButton.Enabled = true;
        }

        private void LimpiarControles()
        {
            IdentidadTextBox.Clear();
            NombreTextBox.Clear();
            TelefonoTextBox.Clear();
            CorreoTextBox.Clear();
            DireccionTextBox.Clear();
            FechaDateTimePicker.Text = "";
            EstaActivoCheckBox.Checked = false;
        }

        private void ClienteForm_Load(object sender, EventArgs e)
        {
            TraerClientes();
        }

        private void TraerClientes()
        {
            ClienteDataGridView.DataSource = clientesDB.DevolverClientes();
        }

        private void NuevoButton_Click(object sender, EventArgs e)
        {
            operacion = "Nuevo";
            HabilitarControles();
        }

        private void ModificarButton_Click(object sender, EventArgs e)
        {
            operacion = "Modificar";

            if (ClienteDataGridView.SelectedRows.Count > 0)
            {
                IdentidadTextBox.Text = ClienteDataGridView.CurrentRow.Cells["Identidad"].Value.ToString();
                NombreTextBox.Text = ClienteDataGridView.CurrentRow.Cells["Nombre"].Value.ToString();
                TelefonoTextBox.Text = ClienteDataGridView.CurrentRow.Cells["Telefono"].Value.ToString();
                CorreoTextBox.Text = ClienteDataGridView.CurrentRow.Cells["Correo"].Value.ToString();
                DireccionTextBox.Text = ClienteDataGridView.CurrentRow.Cells["Direccion"].Value.ToString();
                FechaDateTimePicker.Value = Convert.ToDateTime(ClienteDataGridView.CurrentRow.Cells["FechaNacimiento"].Value);
                EstaActivoCheckBox.Checked = Convert.ToBoolean(ClienteDataGridView.CurrentRow.Cells["EstaActivo"].Value);

                HabilitarControles(); 
                IdentidadTextBox.ReadOnly = true;

            }
            else
            {
                MessageBox.Show("Debe Seleccionar un Registro", " Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GuardarButton_Click(object sender, EventArgs e)
        {
            cliente = new Cliente(); 
             
            cliente.Identidad = IdentidadTextBox.Text;
            cliente.Nombre = NombreTextBox.Text;
            cliente.Telefono = TelefonoTextBox.Text;
            cliente.Correo = CorreoTextBox.Text;
            cliente.Direccion = DireccionTextBox.Text;
            cliente.FechaNacimiento = FechaDateTimePicker.Value;
            cliente.EstaActivo = EstaActivoCheckBox.Checked;

            if (operacion == "Nuevo") 
            {
                if (string.IsNullOrEmpty(IdentidadTextBox.Text))
                {
                    errorProvider1.SetError(IdentidadTextBox, "Ingrese Identidad");
                    IdentidadTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();

                if (string.IsNullOrEmpty(NombreTextBox.Text))
                {
                    errorProvider1.SetError(NombreTextBox, "Ingrese Nombre");
                    NombreTextBox.Focus();
                    return;
                }
                errorProvider1.Clear();

                bool inserto = clientesDB.Insertar(cliente); 
                if (inserto)
                {
                    DeshabilitarControles();
                    LimpiarControles();
                    TraerClientes();
                    MessageBox.Show("Registro Guardado con Exito", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No Se Pudo Guardar el Registro", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (operacion == "Modificar") 
            {
                bool modifico = clientesDB.Editar(cliente);
                if (modifico)
                {
                    IdentidadTextBox.ReadOnly = false; 
                    DeshabilitarControles();
                    LimpiarControles();
                    TraerClientes();
                    MessageBox.Show("Registro actualizado con exito", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se Pudo Actualizar el Registro", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EliminarButton_Click(object sender, EventArgs e)
        {
            if (ClienteDataGridView.SelectedRows.Count > 0)
            {
                DialogResult resultado = MessageBox.Show("Esta seguro de eliminar el registro", "Continúa ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes) 
                {
                    bool elimino = clientesDB.Eliminar(ClienteDataGridView.CurrentRow.Cells["Identidad"].Value.ToString());

                    if (elimino)
                    {
                        LimpiarControles();
                        DeshabilitarControles();
                        TraerClientes();
                        MessageBox.Show("Registro Eliminado", "Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No Se Pudo Eliminar el Registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CancelarButton_Click(object sender, EventArgs e)
        {
            DeshabilitarControles();
            LimpiarControles();
        }
    }
}
