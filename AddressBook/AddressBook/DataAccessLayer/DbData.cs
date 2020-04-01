using AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AddressBook.DataAccessLayer
{
    public class DbData
    {
        public string InsertData(AddressBookModel addressBookModel)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                SqlCommand cmd = new SqlCommand("Usp_InsertAddressBookData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                // i will pass zero to MobileID beacause its Primary .  
                cmd.Parameters.AddWithValue("@name", addressBookModel.Name);
                cmd.Parameters.AddWithValue("@mobileNumber", addressBookModel.MobileNumber);
                //cmd.Parameters.AddWithValue("@Query", 1);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch (Exception ex)
            {
                var x = ex;
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string UpdateData(AddressBookModel addressBookModel)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                SqlCommand cmd = new SqlCommand("Usp_UpdateAddressBookData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", addressBookModel.ID);
                cmd.Parameters.AddWithValue("@Name", addressBookModel.Name);
                cmd.Parameters.AddWithValue("@MobileNumber", addressBookModel.MobileNumber);
                
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string DeleteData(int addressBookId)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                SqlCommand cmd = new SqlCommand("Usp_DeleteAddressBookData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", addressBookId);
                //cmd.Parameters.AddWithValue("@Query", 3);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public IEnumerable<AddressBookModel> PopulateAddressBookData()
        {
            SqlConnection con = null;
            List<AddressBookModel> addressBookList = new List<AddressBookModel>();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                SqlCommand cmd = new SqlCommand("Usp_SelectAllAddressBookData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();


                AddressBookModel addressBook = null;

                while (reader.Read())
                {
                    addressBook = new AddressBookModel();
                    addressBook.ID = int.Parse(reader["ID"].ToString());
                    addressBook.Name = reader["Name"].ToString();
                    addressBook.MobileNumber = reader["MobileNumber"].ToString();
                    addressBookList.Add(addressBook);
                }
                return addressBookList;
            }
            catch (Exception ex)
            {
                var e = ex;
                return addressBookList;
            }
            finally
            {
                con.Close();
            }
        }

        public AddressBookModel GetAddressBookDataByID(int Id)
        {
            SqlConnection con = null;
            AddressBookModel addressBook = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                SqlCommand cmd = new SqlCommand("Usp_SelectAddressBookDataById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    addressBook = new AddressBookModel();
                    addressBook.ID = int.Parse(reader["ID"].ToString());
                    addressBook.Name = reader["Name"].ToString();
                    addressBook.MobileNumber = reader["MobileNumber"].ToString();
                }
                return addressBook;
            }
            catch (Exception ex)
            {
                var e = ex;
                return addressBook;
            }
            finally
            {
                con.Close();
            }
        }


    }

}