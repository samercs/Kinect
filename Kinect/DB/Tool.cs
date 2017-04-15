using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Kinect.DB
{
    public class Tool
    {
        private readonly Database _db;

        public Tool(Database database)
        {
            _db = database;
        }

        public float GetOrderTotal(int reservationId)
        {
            float total = 0;
            _db.AddParameter("@id", reservationId);
            var reservationSeats = _db.ExecuteDataTable("select * from ReservationSeat where ReservationId=@id");
            foreach (DataRow row in reservationSeats.Rows)
            {
                var seatNo = row["seatNo"].ToString();
                switch (seatNo[0])
                {
                    case 'A':
                        total += Prices.APrices;
                        break;
                    case 'B':
                        total += Prices.BPrices;
                        break;
                    case 'C':
                        total += Prices.CPrices;
                        break;
                    case 'D':
                        total += Prices.DPrices;
                        break;
                    case 'E':
                        total += Prices.EPrices;
                        break;
                    default:
                        break;
                }
            }

            _db.AddParameter("@id", reservationId);
            var reservationSnaks = _db.ExecuteDataTable("select * from ReservationSnak where ReservationId=@id");
            foreach (DataRow row in reservationSnaks.Rows)
            {
                var snakName = row["snakName"].ToString();
                var quntity = int.Parse(row["quntity"].ToString());
                var type = row["type"].ToString();

                switch (snakName)
                {
                    case "Popcorn":
                        switch (type)
                        {
                            case "Large":
                                total += quntity * Prices.LargePopcorn;
                                break;
                            case "Medium":
                                total += quntity * Prices.MediumPopcorn;
                                break;
                            case "Small":
                                total += quntity * Prices.SmallPopcorn;
                                break;
                        }
                        break;
                    case "Pepsi":
                        switch (type)
                        {
                            case "Large":
                                total += quntity * Prices.LargePepsi;
                                break;
                            case "Medium":
                                total += quntity * Prices.MediumPepsi;
                                break;
                            case "Small":
                                total += quntity * Prices.SmallPepsi;
                                break;
                        }
                        break;
                    case "Water":
                        switch (type)
                        {
                            case "Large":
                                total += quntity * Prices.LargeWater;
                                break;
                            case "Medium":
                                total += quntity * Prices.MediumWater;
                                break;
                            case "Small":
                                total += quntity * Prices.SmallWater;
                                break;
                        }
                        break;
                    case "Nashos":
                        switch (type)
                        {
                            case "Large":
                                total += quntity * Prices.LargeNashos;
                                break;
                            case "Medium":
                                total += quntity * Prices.MediumNashos;
                                break;
                            case "Small":
                                total += quntity * Prices.SmallNashos;
                                break;
                        }
                        break;
                }

            }

            return total;
        }
    }
}
