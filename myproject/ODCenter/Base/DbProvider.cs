using ODCenter.Models;
using PTR.Lbs.Baidu;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Base
{
    public static class DbProvider
    {
        private static Object obj = new Object();

        #region Institutes Extensions

        public static SortedList<String, Institute> Institutes = null;

        public static IEnumerable<InstituteInfo> All(this SortedList<String, Institute> target)
        {
            using (var db_ins = new InstituteDbContext())
            {
                return db_ins.Institutes.ToList();
            }
        }

        public static IEnumerable<SelectListItem> List(this SortedList<String, Institute> target, Guid? insid)
        {
            return (from ins in Institutes.Values
                    select new SelectListItem()
                    {
                        Text = ins.Name,
                        Value = ins.Id.ToString("N"),
                        Selected = (ins.Id == insid)
                    });
        }

        public static InstituteInfo Find(this SortedList<String, Institute> target, Guid? id)
        {
            using (var db_ins = new InstituteDbContext())
            {
                return db_ins.Institutes.Find(id);
            }
        }

        public static Guid Create(this SortedList<String, Institute> target, InstituteInfo institute, String user)
        {
            using (var db_ins = new InstituteDbContext())
            {
                institute.Id = Guid.NewGuid();
                institute.Address = target.Locate(institute);
                institute.Enabled = true;
                institute.Modifier = user;
                db_ins.Institutes.Add(institute);
                InstituteLog log = db_ins.CreateLog(institute);
                if (log != null)
                {
                    db_ins.Logs.Add(log);
                }
                db_ins.SaveChanges();
                institute = db_ins.Institutes.Find(institute.Id);
                if (institute != null)
                {
                    Institutes.Add(institute.Id.ToString("N"), new Institute(institute));
                }
                return institute.Id;
            }
        }

        public static String Locate(this SortedList<String, Institute> target, InstituteInfo institute)
        {
            if (!institute.Latitude.HasValue || !institute.Longitude.HasValue)
            {
                return null;
            }
            return target.Locate((Decimal)institute.Longitude.Value, (Decimal)institute.Latitude.Value);
        }

        public static String Locate(this SortedList<String, Institute> target, Decimal longitude, Decimal latitude)
        {
            try
            {
                GeocodingResults res = LbsGeo.Coding(
                    new LbsLocation()
                    {
                        Longitude = longitude,
                        Latitude = latitude
                    }, LbsCoordType.wgs84ll);
                if (res.Status == LbsServiceStatus.Success && res.Result != null)
                {
                    return res.Result.FormattedAddress;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when retrieve address.", ex);
            }
            return null;
        }

        public static Boolean Update(this SortedList<String, Institute> target, InstituteInfo institute_new, String user)
        {
            using (var db_ins = new InstituteDbContext())
            {
                var institute = db_ins.Institutes.Find(institute_new.Id);
                if (institute == null || institute.Equals(institute_new))
                {
                    return false;
                }
                InstituteLog log = db_ins.EditLog(institute, institute_new);
                if (log != null)
                {
                    db_ins.Logs.Add(log);
                }
                institute.Name = institute_new.Name;
                institute.Type = institute_new.Type;
                institute.Location = institute_new.Location;
                if (institute.Longitude != institute_new.Longitude || institute.Latitude != institute_new.Latitude)
                {
                    institute.Longitude = institute_new.Longitude;
                    institute.Latitude = institute_new.Latitude;
                    institute.Address = target.Locate(institute);
                }
                institute.Address = institute_new.Address;
                institute.Summary = institute_new.Summary;
                institute.Modifier = user;
                db_ins.Entry(institute).State = EntityState.Modified;
                db_ins.SaveChanges();

                if (Institutes.ContainsKey(institute.Id.ToString("N")))
                {
                    Institute ins = Institutes[institute.Id.ToString("N")];
                    ins.Name = institute_new.Name;
                    ins.Type = institute_new.Type;
                    ins.Location = institute_new.Location;
                }
                return true;
            }
        }

        public static Boolean Move(this SortedList<String, Institute> target, Guid insid, Guid clientid, Double? longitude, Double? latitude)
        {
            using (var db_ins = new InstituteDbContext())
            {
                if (insid == null || insid == Guid.Empty)
                {
                    var ins = Institutes.Values.FirstOrDefault(i => i.Clients.ContainsKey(clientid.ToString("N")));
                    if (ins != null)
                    {
                        insid = ins.Id;
                    }
                }
                var institute = db_ins.Institutes.Find(insid);
                if (institute == null)
                {
                    return false;
                }
                if (institute.Longitude == longitude && institute.Latitude == latitude)
                {
                    return true;
                }
                institute.Longitude = longitude;
                institute.Latitude = latitude;
                institute.Address = target.Locate(institute);
                db_ins.Entry(institute).State = EntityState.Modified;

                db_ins.Locations.Add(new InstituteLocation()
                {
                    Id = institute.Id,
                    Time = DateTime.Now,
                    Longitude = longitude,
                    Latitude = latitude,
                    Address = institute.Address
                });
                db_ins.SaveChanges();

                String sins = institute.Id.ToString("N");
                if (Institutes.ContainsKey(sins))
                {
                    Institutes[sins].Longitude = institute.Longitude;
                    Institutes[sins].Latitude = institute.Latitude;
                    Institutes[sins].Address = institute.Address;
                }
            }
            return true;
        }

        public static InstituteInfo Delete(this SortedList<String, Institute> target, Guid institute_id, String user)
        {
            using (var db_ins = new InstituteDbContext())
            {
                var institute = db_ins.Institutes.Find(institute_id);
                if (institute == null || !institute.Enabled)
                {
                    return null;
                }
                institute.Enabled = false;
                institute.Modifier = user;
                db_ins.Entry(institute).State = EntityState.Modified;
                InstituteLog log = db_ins.DeleteLog(institute);
                if (log != null)
                {
                    db_ins.Logs.Add(log);
                }
                db_ins.SaveChanges();
                Institutes.Remove(institute_id.ToString("N"));
                return institute;
            }
        }

        #endregion



        #region Clients Extensions

        public static SortedList<String, Client> Clients = null;

        public static IEnumerable<ClientInfo> All(this SortedList<String, Client> target)
        {
            using (var db_cli = new ClientDbContext())
            {
                return db_cli.Clients.ToList();
            }
        }

        public static Client Find(this SortedList<String, Client> target, Int32 index)
        {
            foreach (Client client in Clients.Values)
            {
                if (client.Index == index)
                {
                    return client;
                }
            }
            return null;
        }

        public static ClientInfo Find(this SortedList<String, Client> target, Guid? id)
        {
            using (var db_cli = new ClientDbContext())
            {
                return db_cli.Clients.Find(id);
            }
        }

        public static Boolean Create(this SortedList<String, Client> target, ClientInfo client, Guid institute_id, String user)
        {
            using (var db_cli = new ClientDbContext())
            {
                client.Id = Guid.NewGuid();
                client.Institute = institute_id;
                client.Enabled = true;
                client.LastActive = new DateTime(1900, 1, 1);
                client.Modifier = user;
                db_cli.Clients.Add(client);
                ClientLog log = db_cli.CreateLog(client);
                if (log != null)
                {
                    db_cli.Logs.Add(log);
                }
                db_cli.SaveChanges();

                String cliid = client.Id.ToString("N");
                Clients.Add(cliid, new Client(client));
                String insid = client.Institute.ToString("N");
                if (Institutes.ContainsKey(insid))
                {
                    Institutes[insid].Clients.Add(cliid, Clients[cliid]);
                }
                return true;
            }
        }

        public static Boolean Update(this SortedList<String, Client> target, ClientInfo client_new, String user)
        {
            using (var db_cli = new ClientDbContext())
            {
                var client = db_cli.Clients.Find(client_new.Id);
                if (client == null || client.Equals(client_new))
                {
                    return false;
                }
                ClientLog log = db_cli.EditLog(client, client_new);
                if (log != null)
                {
                    db_cli.Logs.Add(log);
                }
                client.Name = client_new.Name;
                client.Index = client_new.Index;
                client.Dtu = client_new.Dtu;
                if (client.Longitude != client_new.Longitude || client.Latitude != client_new.Latitude)
                {
                    client.Longitude = client_new.Longitude;
                    client.Latitude = client_new.Latitude;
                    Institutes.Move(Guid.Empty, client.Id, client.Longitude, client.Latitude);
                }
                client.Modifier = user;
                db_cli.Entry(client).State = EntityState.Modified;
                db_cli.SaveChanges();

                if (Clients.ContainsKey(client.Id.ToString("N")))
                {
                    Client cli = Clients[client.Id.ToString("N")];
                    cli.Name = client_new.Name;
                    cli.Index = client_new.Index;
                    cli.Dtu = client_new.Dtu;
                    cli.Invalidate();
                }
            }
            return false;
        }

        public static Boolean Move(this SortedList<String, Client> target, Guid client_id, Double? longitude, Double? latitude)
        {
            using (var db_cli = new ClientDbContext())
            {
                var client = db_cli.Clients.Find(client_id);
                if (client == null)
                {
                    return false;
                }
                if (client.Longitude == longitude && client.Latitude == latitude)
                {
                    return true;
                }
                client.Longitude = longitude;
                client.Latitude = latitude;
                db_cli.Entry(client).State = EntityState.Modified;

                db_cli.Locations.Add(new ClientLocation()
                {
                    Id = client.Id,
                    Time = DateTime.Now,
                    Longitude = longitude,
                    Latitude = latitude
                });
                db_cli.SaveChanges();

                Institutes.Move(Guid.Empty, client_id, longitude, latitude);

                String scli = client.Id.ToString("N");
                if (Clients.ContainsKey(scli))
                {
                    Clients[scli].Longitude = longitude;
                    Clients[scli].Latitude = latitude;
                }
                return true;
            }
        }

        public static Boolean Delete(this SortedList<String, Client> target, Guid client_id, String user)
        {
            using (var db_cli = new ClientDbContext())
            {
                var client = db_cli.Clients.Find(client_id);
                if (client == null)
                {
                    return false;
                }
                client.Enabled = false;
                client.Modifier = user;
                db_cli.Entry(client).State = EntityState.Modified;
                ClientLog log = db_cli.DeleteLog(client);
                if (log != null)
                {
                    db_cli.Logs.Add(log);
                }
                db_cli.SaveChanges();

                String cliid = client.Id.ToString("N");
                String insid = client.Institute.ToString("N");
                if (Institutes.ContainsKey(insid))
                {
                    Institutes[insid].Clients.Remove(cliid);
                }
                Clients.Remove(cliid);
                return true;
            }
        }

        public static IEnumerable<SelectListItem> SensorsToConnect(this SortedList<String, Client> target, Guid? institute_id, Boolean orphanonly)
        {
            List<Guid> senids = new List<Guid>();
            if (institute_id.HasValue && Institutes.ContainsKey(institute_id.Value.ToString("N")))
            {
                foreach (Sensor s in Sensors.Values)
                {
                    if (s.Institute == institute_id)
                    {
                        senids.Add(s.Id);
                    }
                }
                if (orphanonly)
                {
                    foreach (Client c in Institutes[institute_id.Value.ToString("N")].Clients.Values)
                    {
                        foreach (SensorEx s in c.Sensors.Values)
                        {
                            senids.Remove(s.Sensor.Id);
                        }
                    }
                }
                return (from sen in Sensors.Values
                        where senids.Contains(sen.Id)
                        select new SelectListItem()
                        {
                            Text = sen.Name,
                            Value = sen.Id.ToString("N")
                        });
            }
            return null;
        }

        public static Boolean ConnectSensor(this SortedList<String, Client> target, Guid client_id, Guid sensor_id, Int32? sensor_index = null)
        {
            using (var db_cli = new ClientDbContext())
            {
                ClientConn conn = db_cli.Conns.Find(client_id, sensor_id);
                String cli = client_id.ToString("N"), sen = sensor_id.ToString("N");
                if (conn == null)
                {
                    db_cli.Conns.Add(new ClientConn(client_id, sensor_id, sensor_index));
                    db_cli.SaveChanges();

                    if (Clients.ContainsKey(cli) && Sensors.ContainsKey(sen))
                    {
                        Clients[cli].Sensors.Add(sen, new SensorEx(Sensors[sen], sensor_index));
                        Clients[cli].Invalidate();
                    }
                }
                else
                {
                    if (conn.SensorIndex == sensor_index)
                    {
                        return true;
                    }
                    conn.SensorIndex = sensor_index;
                    db_cli.SaveChanges();
                    if (Clients.ContainsKey(cli) && Clients[cli].Sensors.ContainsKey(sen))
                    {
                        Clients[cli].Sensors[sen].Index = sensor_index;
                        Clients[cli].Invalidate();
                    }
                }
                return false;
            }
        }

        public static Boolean DisconnectSensor(this SortedList<String, Client> target, Guid client_id, Guid sensor_id)
        {
            using (var db_cli = new ClientDbContext())
            {
                ClientConn conn = db_cli.Conns.Find(client_id, sensor_id);
                if (conn != null)
                {
                    db_cli.Conns.Remove(conn);
                    db_cli.SaveChanges();

                    String cli = client_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Clients.ContainsKey(cli) && Clients[cli].Sensors.ContainsKey(sen))
                    {
                        Clients[cli].Sensors.Remove(sen);
                        Clients[cli].Invalidate();
                    }
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region OPCClients Extensions
        public static SortedList<String, Client> OPCClients() {
            SortedList<String, Client> opcclients = new SortedList<string, Client>();
            foreach (Client client in Clients.Values)
            {
                String insid = client.Institute.ToString("N");
                if (Institutes.ContainsKey(insid))
                {
                    if (Institutes[insid].Type == Ins_Type.Normal) {
                        String opcstr = client.Id.ToString("N");
                        opcclients.Add(opcstr, client);
                    }
                    
                }  
            }
            return opcclients;
        }
        #endregion

        #region Devices Extensions

        public static SortedList<String, Device> Devices = null;

        public static IEnumerable<DeviceInfo> All(this SortedList<String, Device> target)
        {
            using (var db_dev = new DeviceDbContext())
            {
                return db_dev.Devices.ToList();
            }
        }

        public static DeviceInfo Find(this SortedList<String, Device> target, Guid? id)
        {
            using (var db_dev = new DeviceDbContext())
            {
                return db_dev.Devices.Find(id);
            }
        }

        public static Boolean Create(this SortedList<String, Device> target, DeviceInfo device, Guid institute_id, String user)
        {
            using (var db_dev = new DeviceDbContext())
            {
                device.Id = Guid.NewGuid();
                device.Institute = institute_id;
                device.Enabled = true;
                device.Modifier = user;
                db_dev.Devices.Add(device);
                DeviceLog log = db_dev.CreateLog(device);
                if (log != null)
                {
                    db_dev.Logs.Add(log);
                }
                DeviceStat stat = db_dev.EditLog(device, device.Status, Dev_Stat.Manual);
                if (stat != null)
                {
                    db_dev.Stats.Add(stat);
                }
                db_dev.SaveChanges();
                Devices.Add(device.Id.ToString("N"), new Device(device));
                return true;
            }
        }

        public static Boolean Update(this SortedList<String, Device> target, DeviceInfo device_new, String user)
        {
            using (var db_dev = new DeviceDbContext())
            {
                DeviceInfo device = db_dev.Devices.Find(device_new.Id);
                if (device == null || device.Equals(device_new))
                {
                    return false;
                }
                DeviceLog log = db_dev.EditLog(device, device_new);
                if (log != null)
                {
                    db_dev.Logs.Add(log);
                }
                device.Name = device_new.Name;
                device.Statsrc = device_new.Statsrc;
                if (device.Status != device_new.Status)
                {
                    DeviceStat stat = db_dev.EditLog(device, device_new.Status, Dev_Stat.Manual);
                    if (stat != null)
                    {
                        db_dev.Stats.Add(stat);
                    }
                    device.Status = device_new.Status;
                }
                db_dev.Entry(device).State = EntityState.Modified;
                db_dev.SaveChanges();

                if (Devices.ContainsKey(device.Id.ToString("N")))
                {
                    Device dev = Devices[device.Id.ToString("N")];
                    dev.Name = device_new.Name;
                    dev.Statsrc = device_new.Statsrc;
                    dev.Status = device_new.Status;
                }
                return true;
            }
        }

        public static Boolean Update(this SortedList<String, Device> target, Guid device_id)
        {
            Device dev = Devices[device_id.ToString("N")];
            if (dev == null)
            {
                return false;
            }
            String status = dev.Status;
            dev.UpdateStatus();
            if (status == dev.Status)
            {
                return true;
            }
            using (var db_dev = new DeviceDbContext())
            {
                DeviceInfo devinfo = db_dev.Devices.Find(device_id);
                if (devinfo == null || devinfo.Status == dev.Status)
                {
                    return false;
                }
                DeviceStat stat = db_dev.EditLog(devinfo, dev.Status, Dev_Stat.Automatic);
                if (stat != null)
                {
                    db_dev.Stats.Add(stat);
                }
                devinfo.Status = dev.Status;
                db_dev.Entry(devinfo).State = EntityState.Modified;
                db_dev.SaveChanges();
                return true;
            }
        }

        public static Boolean Delete(this SortedList<String, Device> target, Guid device_id, String user)
        {
            using (var db_dev = new DeviceDbContext())
            {
                DeviceInfo device = db_dev.Devices.Find(device_id);
                if (device == null)
                {
                    return false;
                }
                device.Enabled = false;
                device.Modifier = user;
                db_dev.Entry(device).State = EntityState.Modified;
                DeviceLog log = db_dev.DeleteLog(device);
                if (log != null)
                {
                    db_dev.Logs.Add(log);
                }
                db_dev.SaveChanges();

                String devid = device.Id.ToString("N");
                foreach (Group g in Groups.Values)
                {
                    if (g.Devices.ContainsKey(devid))
                    {
                        g.Devices.Remove(devid);
                    }
                }
                Devices.Remove(devid);
                return true;
            }
        }

        public static Boolean TrackAppend(this SortedList<String, Device> target, DeviceTrack track)
        {
            using (var db_dev = new DeviceDbContext())
            {
                db_dev.Tracks.Add(track);
                db_dev.SaveChanges();
                return true;
            }
        }

        public static Boolean TrackUpdate(this SortedList<String, Device> target, DeviceTrack track_new)
        {
            using (var db_dev = new DeviceDbContext())
            {
                var track = db_dev.Tracks.Find(track_new.Id);
                if (track == null)
                {
                    return false;
                }
                DeviceLog log = db_dev.EditLog(track, track_new);
                if (log != null)
                {
                    db_dev.Logs.Add(log);
                }
                track.Description = track_new.Description;
                db_dev.Entry(track).State = EntityState.Modified;
                db_dev.SaveChanges();
                return true;
            }
        }

        public static Boolean TrackDelete(this SortedList<String, Device> target, Guid track_id)
        {
            using (var db_dev = new DeviceDbContext())
            {
                var track = db_dev.Tracks.Find(track_id);
                if (track == null)
                {
                    return false;
                }
                track.Enabled = false;
                db_dev.Entry(track).State = EntityState.Modified;
                db_dev.SaveChanges();
                return true;
            }
        }

        public static IEnumerable<SelectListItem> SensorsToConnect(this SortedList<String, Device> target, Guid? institute_id, Boolean orphanonly)
        {
            List<Guid> senids = new List<Guid>();
            if (institute_id.HasValue && Institutes.ContainsKey(institute_id.Value.ToString("N")))
            {
                foreach (Sensor s in Sensors.Values)
                {
                    if (s.Institute == institute_id)
                    {
                        senids.Add(s.Id);
                    }
                }
                if (orphanonly)
                {
                    foreach (Group g in Institutes[institute_id.Value.ToString("N")].Groups.Values)
                    {
                        foreach (Device d in g.Devices.Values)
                        {
                            foreach (Sensor s in d.Sensors.Values)
                            {
                                senids.Remove(s.Id);
                            }
                        }
                    }
                }
                return (from sen in Sensors.Values
                        where senids.Contains(sen.Id)
                        select new SelectListItem()
                        {
                            Text = sen.Name,
                            Value = sen.Id.ToString("N")
                        });
            }
            return null;
        }

        public static Boolean ConnectSensor(this SortedList<String, Device> target, Guid device_id, Guid sensor_id)
        {
            using (var db_sen = new SensorDbContext())
            {
                SensorInfo seninfo = db_sen.Sensors.Find(sensor_id);
                if (seninfo != null)
                {
                    seninfo.Device = device_id;
                    db_sen.Entry(seninfo).State = EntityState.Modified;
                    db_sen.SaveChanges();

                    String dev = device_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Devices.ContainsKey(dev) && Sensors.ContainsKey(sen))
                    {
                        Devices[dev].Sensors.Add(sen, Sensors[sen]);
                        Sensors[sen].Device = device_id;
                    }
                    return true;
                }
                return false;
            }
        }

        public static Boolean DisconnectSensor(this SortedList<String, Device> target, Guid device_id, Guid sensor_id)
        {
            using (var db_sen = new SensorDbContext())
            {
                SensorInfo seninfo = db_sen.Sensors.Find(sensor_id);
                if (seninfo != null && seninfo.Device == device_id)
                {
                    seninfo.Device = null;
                    db_sen.Entry(seninfo).State = EntityState.Modified;
                    db_sen.SaveChanges();
                    String dev = device_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Devices.ContainsKey(dev))
                    {
                        Devices[dev].Sensors.Remove(sen);
                    }
                    if (Sensors.ContainsKey(sen))
                    {
                        Sensors[sen].Device = null;
                    }
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region Groups Extensions

        public static SortedList<String, Group> Groups = null;

        public static IEnumerable<GroupInfo> All(this SortedList<String, Group> target)
        {
            using (var db_grp = new GroupDbContext())
            {
                return db_grp.Groups.ToList();
            }
        }

        public static GroupInfo Find(this SortedList<String, Group> target, Guid? id)
        {
            using (var db_grp = new GroupDbContext())
            {
                return db_grp.Groups.Find(id);
            }
        }

        public static Boolean Create(this SortedList<String, Group> target, GroupInfo group, Guid institute_id, String user)
        {
            using (var db_grp = new GroupDbContext())
            {
                group.Id = Guid.NewGuid();
                group.Institute = institute_id;
                group.Enabled = true;
                group.Modifier = user;
                db_grp.Groups.Add(group);
                GroupLog log = db_grp.CreateLog(group);
                if (log != null)
                {
                    db_grp.Logs.Add(log);
                }
                db_grp.SaveChanges();

                String grpid = group.Id.ToString("N");
                Groups.Add(grpid, new Group(group));
                String insid = group.Institute.ToString("N");
                if (Institutes.ContainsKey(insid))
                {
                    Institutes[insid].Groups.Add(grpid, Groups[grpid]);
                }
                return true;
            }
        }

        public static Boolean Update(this SortedList<String, Group> target, GroupInfo group_new, String user)
        {
            using (var db_grp = new GroupDbContext())
            {
                var group = db_grp.Groups.Find(group_new.Id);
                if (group == null || group.Equals(group_new))
                {
                    return false;
                }
                GroupLog log = db_grp.EditLog(group, group_new);
                if (log != null)
                {
                    db_grp.Logs.Add(log);
                }
                group.Name = group_new.Name;
                group.Modifier = user;
                db_grp.Entry(group).State = EntityState.Modified;
                db_grp.SaveChanges();

                if (Groups.ContainsKey(group.Id.ToString("N")))
                {
                    Group grp = Groups[group.Id.ToString("N")];
                    grp.Name = group_new.Name;
                }
            }
            return false;
        }

        public static Boolean Delete(this SortedList<String, Group> target, Guid group_id, String user)
        {
            using (var db_grp = new GroupDbContext())
            {
                var group = db_grp.Groups.Find(group_id);
                if (group == null)
                {
                    return false;
                }
                group.Enabled = false;
                group.Modifier = user;
                db_grp.Entry(group).State = EntityState.Modified;
                GroupLog log = db_grp.DeleteLog(group);
                if (log != null)
                {
                    db_grp.Logs.Add(log);
                }
                db_grp.SaveChanges();

                Groups.Remove(group.Id.ToString("N"));
            }
            return false;
        }

        public static IEnumerable<SelectListItem> DevicesToConnect(this SortedList<String, Group> target, Guid? institute_id, Boolean orphanonly)
        {
            List<Guid> devids = new List<Guid>();
            if (institute_id.HasValue && Institutes.ContainsKey(institute_id.Value.ToString("N")))
            {
                foreach (Device d in Devices.Values)
                {
                    if (d.Institute == institute_id)
                    {
                        devids.Add(d.Id);
                    }
                }
                if (orphanonly)
                {
                    foreach (Group g in Institutes[institute_id.Value.ToString("N")].Groups.Values)
                    {
                        foreach (Device d in g.Devices.Values)
                        {
                            devids.Remove(d.Id);
                        }
                    }
                }
                return (from dev in Devices.Values
                        where devids.Contains(dev.Id)
                        select new SelectListItem()
                        {
                            Text = dev.Name,
                            Value = dev.Id.ToString("N")
                        });
            }
            return null;
        }

        public static Boolean ConnectDevice(this SortedList<String, Group> target, Guid group_id, Guid device_id)
        {
            using (var db_dev = new DeviceDbContext())
            {
                if (db_dev.Conns.Find(group_id, device_id) == null)
                {
                    db_dev.Conns.Add(new DeviceConn(group_id, device_id));
                    db_dev.SaveChanges();

                    String grp = group_id.ToString("N"), dev = device_id.ToString("N");
                    if (Groups.ContainsKey(grp) && Devices.ContainsKey(dev))
                    {
                        Groups[grp].Devices.Add(dev, Devices[dev]);
                    }
                    return true;
                }
            }
            return false;
        }

        public static Boolean DisconnectDevice(this SortedList<String, Group> target, Guid group_id, Guid device_id)
        {
            using (var db_dev = new DeviceDbContext())
            {
                DeviceConn conn = db_dev.Conns.Find(group_id, device_id);
                if (conn != null)
                {
                    db_dev.Conns.Remove(conn);
                    db_dev.SaveChanges();

                    String grp = group_id.ToString("N"), dev = device_id.ToString("N");
                    if (Groups.ContainsKey(grp) && Groups[grp].Devices.ContainsKey(dev))
                    {
                        Groups[grp].Devices.Remove(dev);
                    }
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<SelectListItem> SensorsToConnect(this SortedList<String, Group> target, Guid? institute_id, Boolean orphanonly)
        {
            List<Guid> senids = new List<Guid>();
            if (institute_id.HasValue && Institutes.ContainsKey(institute_id.Value.ToString("N")))
            {
                foreach (Sensor s in Sensors.Values)
                {
                    if (s.Institute == institute_id)
                    {
                        senids.Add(s.Id);
                    }
                }
                if (orphanonly)
                {
                    foreach (Group g in Institutes[institute_id.Value.ToString("N")].Groups.Values)
                    {
                        foreach (Sensor s in g.Sensors.Values)
                        {
                            senids.Remove(s.Id);
                        }
                    }
                }
                return (from sen in Sensors.Values
                        where senids.Contains(sen.Id)
                        select new SelectListItem()
                        {
                            Text = sen.Name,
                            Value = sen.Id.ToString("N")
                        });
            }
            return null;
        }

        public static Boolean ConnectSensor(this SortedList<String, Group> target, Guid group_id, Guid sensor_id, Boolean visible)
        {
            using (var db_sen = new SensorDbContext())
            {
                if (db_sen.Conns.Find(group_id, sensor_id) == null)
                {
                    db_sen.Conns.Add(new SensorConn(group_id, sensor_id, visible));
                    db_sen.SaveChanges();

                    String group = group_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Groups.ContainsKey(group) && Sensors.ContainsKey(sen))
                    {
                        Groups[group].Sensors.Add(sen, Sensors[sen]);
                        if (visible)
                        {
                            Groups[group].Visibles.Add(sensor_id);
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public static Boolean ShowSensor(this SortedList<String, Group> target, Guid group_id, Guid sensor_id)
        {
            using (var db_sen = new SensorDbContext())
            {
                SensorConn conn = db_sen.Conns.Find(group_id, sensor_id);
                if (conn != null)
                {
                    conn.Visible = true;
                    db_sen.Entry(conn).State = EntityState.Modified;
                    db_sen.SaveChanges();

                    String group = group_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Groups.ContainsKey(group) && !Groups[group].Visibles.Contains(sensor_id))
                    {
                        Groups[group].Visibles.Add(sensor_id);
                    }
                    return true;
                }
                return false;
            }
        }

        public static Boolean HideSensor(this SortedList<String, Group> target, Guid group_id, Guid sensor_id)
        {
            using (var db_sen = new SensorDbContext())
            {
                SensorConn conn = db_sen.Conns.Find(group_id, sensor_id);
                if (conn != null)
                {
                    conn.Visible = false;
                    db_sen.Entry(conn).State = EntityState.Modified;
                    db_sen.SaveChanges();

                    String group = group_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Groups.ContainsKey(group) && Groups[group].Visibles.Contains(sensor_id))
                    {
                        Groups[group].Visibles.Remove(sensor_id);
                    }
                    return true;
                }
                return false;
            }
        }

        public static Boolean DisconnectSensor(this SortedList<String, Group> target, Guid group_id, Guid sensor_id)
        {
            using (var db_sen = new SensorDbContext())
            {
                SensorConn conn = db_sen.Conns.Find(group_id, sensor_id);
                if (conn != null)
                {
                    db_sen.Conns.Remove(conn);
                    db_sen.SaveChanges();

                    String group = group_id.ToString("N"), sen = sensor_id.ToString("N");
                    if (Groups.ContainsKey(group) && Groups[group].Sensors.ContainsKey(sen))
                    {
                        Groups[group].Sensors.Remove(sen);
                    }
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Sensors Extensions

        public static SortedList<String, Sensor> Sensors = null;

        public static IEnumerable<SensorInfo> All(this SortedList<String, Sensor> target)
        {
            using (var db_sen = new SensorDbContext())
            {
                return db_sen.Sensors.ToList();
            }
        }

        public static SensorInfo Find(this SortedList<String, Sensor> target, Guid? sensor_id)
        {
            using (var db_sen = new SensorDbContext())
            {
                return db_sen.Sensors.Find(sensor_id);
            }
        }

        public static Boolean Create(this SortedList<String, Sensor> target, SensorInfo sensor, Guid institute_id, String user)
        {
            using (var db_sen = new SensorDbContext())
            {
                sensor.Id = Guid.NewGuid();
                sensor.Institute = institute_id;
                sensor.Enabled = true;
                sensor.Modifier = user;
                db_sen.Sensors.Add(sensor);
                SensorLog log = db_sen.CreateLog(sensor);
                if (log != null)
                {
                    db_sen.Logs.Add(log);
                }
                db_sen.SaveChanges();

                String senid = sensor.Id.ToString("N");
                Sensors.Add(senid, new Sensor(sensor));
                if (sensor.Device.HasValue)
                {
                    Devices.ConnectSensor(sensor.Device.Value, sensor.Id);
                }
                return true;
            }
        }

        public static Boolean Update(this SortedList<String, Sensor> target, SensorInfo sensor_new, String user)
        {
            using (var db_sen = new SensorDbContext())
            {
                SensorInfo sensor = db_sen.Sensors.Find(sensor_new.Id);
                if (sensor == null || sensor.Equals(sensor_new))
                {
                    return false;
                }
                SensorLog log = db_sen.EditLog(sensor, sensor_new);
                if (log != null)
                {
                    db_sen.Logs.Add(log);
                }
                sensor.Name = sensor_new.Name;
                sensor.DatType = sensor_new.DatType;
                sensor.SenType = sensor_new.SenType;
                sensor.Inverse = sensor_new.Inverse;
                sensor.Precise = sensor_new.Precise;
                sensor.DatSource = sensor_new.DatSource;
                sensor.OPCHost = sensor_new.OPCHost;
                sensor.OPCServer = sensor_new.OPCServer;
                sensor.OPCItem = sensor_new.OPCItem;
                sensor.Interval = sensor_new.Interval;
                sensor.Gain = sensor_new.Gain;
                sensor.Offset = sensor_new.Offset;
                sensor.Unit = sensor_new.Unit;
                sensor.Order = sensor_new.Order;
                sensor.Important = sensor_new.Important;
                sensor.Modifier = user;
                db_sen.Entry(sensor).State = EntityState.Modified;
                db_sen.SaveChanges();

                String senid = sensor.Id.ToString("N");
                Sensor sen = DbProvider.Sensors[senid];
                if (sen != null)
                {
                    sen.Name = sensor_new.Name;
                    sen.DatType = sensor_new.DatType;
                    sen.SenType = sensor_new.SenType;
                    sen.Inverse = sensor_new.Inverse;
                    sen.Precise = sensor_new.Precise;
                    sen.Source = sensor_new.DatSource;
                    sen.OPCHost = sensor_new.OPCHost;
                    sen.OPCServer = sensor_new.OPCServer;
                    sen.OPCItem = sensor_new.OPCItem;
                    sen.Interval = sensor_new.Interval;
                    sen.Gain = sensor_new.Gain;
                    sen.Offset = sensor_new.Offset;
                    sen.Unit = sensor_new.Unit;
                    sen.Order = sensor_new.Order;
                    sen.Important = sensor_new.Important;
                    foreach (Client cli in Clients.Values)
                    {
                        if (cli.Sensors.ContainsKey(senid))
                        {
                            cli.Invalidate();
                        }
                    }
                }
                return true;
            }
        }

        public static Boolean Delete(this SortedList<String, Sensor> target, Guid sensor_id, String user)
        {
            using (var db_sen = new SensorDbContext())
            {
                var sensor = db_sen.Sensors.Find(sensor_id);
                if (sensor == null)
                {
                    return false;
                }
                sensor.Enabled = false;
                sensor.Modifier = user;
                db_sen.Entry(sensor).State = EntityState.Modified;
                SensorLog log = db_sen.DeleteLog(sensor);
                if (log != null)
                {
                    db_sen.Logs.Add(log);
                }
                db_sen.SaveChanges();

                String senid = sensor.Id.ToString("N");
                foreach (Client cli in Clients.Values)
                {
                    if (cli.Sensors.ContainsKey(senid))
                    {
                        cli.Sensors.Remove(senid);
                        cli.Invalidate();
                    }
                }
                if (sensor.Device.HasValue && Devices.ContainsKey(sensor.Device.Value.ToString("N")))
                {
                    Devices[sensor.Device.Value.ToString("N")].Sensors.Remove(senid);
                }
                Sensors.Remove(senid);
            }
            return false;
        }

        #endregion

        public static Boolean Load()
        {
            try
            {
                #region Institute
                using (InstituteDbContext db_ins = new InstituteDbContext())
                {
                    Institutes = new SortedList<String, Institute>();
                    foreach (InstituteInfo ins in db_ins.Institutes)
                    {
                        if (ins.Enabled)
                        {
                            Institutes.Add(ins.Id.ToString("N"), new Institute(ins));
                        }
                    }
                }
                #endregion

                #region Group
                using (GroupDbContext db_grp = new GroupDbContext())
                {
                    Groups = new SortedList<String, Group>();
                    foreach (GroupInfo grp in db_grp.Groups)
                    {
                        if (grp.Enabled)
                        {
                            Groups.Add(grp.Id.ToString("N"), new Group(grp));
                        }
                    }
                    foreach (String grpid in Groups.Keys)
                    {
                        Group grp = Groups[grpid];
                        String insid = grp.Institute.ToString("N");
                        if (Institutes.ContainsKey(insid))
                        {
                            Institutes[insid].Groups.Add(grpid, grp);
                        }
                    }
                }
                #endregion

                #region Sensor
                using (SensorDbContext db_sen = new SensorDbContext())
                {
                    Sensors = new SortedList<String, Sensor>();
                    foreach (SensorInfo sen in db_sen.Sensors)
                    {
                        if (sen.Enabled)
                        {
                            Sensors.Add(sen.Id.ToString("N"), new Sensor(sen));
                        }
                    }
                    foreach (SensorConn con in db_sen.Conns)
                    {
                        String grpid = con.GroupId.ToString("N");
                        String senid = con.SensorId.ToString("N");
                        if (Sensors.ContainsKey(senid) && Groups.ContainsKey(grpid))
                        {
                            Groups[grpid].Sensors.Add(senid, Sensors[senid]);
                            if (con.Visible)
                            {
                                Groups[grpid].Visibles.Add(con.SensorId);
                            }
                        }
                    }
                }
                #endregion

                #region Device
                using (DeviceDbContext db_dev = new DeviceDbContext())
                {
                    Devices = new SortedList<String, Device>();
                    foreach (DeviceInfo dev in db_dev.Devices)
                    {
                        if (dev.Enabled)
                        {
                            Devices.Add(dev.Id.ToString("N"), new Device(dev));
                        }
                    }
                    foreach (DeviceConn con in db_dev.Conns)
                    {
                        String grpid = con.GroupId.ToString("N");
                        String devid = con.DeviceId.ToString("N");
                        if (Devices.ContainsKey(devid) && Groups.ContainsKey(grpid))
                        {
                            Groups[grpid].Devices.Add(devid, Devices[devid]);
                        }
                    }
                    foreach (String senid in Sensors.Keys)
                    {
                        Sensor sen = Sensors[senid];
                        if (sen.Device.HasValue)
                        {
                            String devid = sen.Device.Value.ToString("N");
                            if (Devices.ContainsKey(devid))
                            {
                                Devices[devid].Sensors.Add(senid, sen);
                            }
                        }
                    }
                }
                #endregion

                #region Client
                using (ClientDbContext db_cli = new ClientDbContext())
                {
                    Clients = new SortedList<String, Client>();
                    foreach (ClientInfo cli in db_cli.Clients)
                    {
                        if (cli.Enabled)
                        {
                            Clients.Add(cli.Id.ToString("N"), new Client(cli));
                        }
                    }
                    foreach (ClientConn con in db_cli.Conns)
                    {
                        String cliid = con.ClientId.ToString("N");
                        String senid = con.SensorId.ToString("N");
                        if (Sensors.ContainsKey(senid) && Clients.ContainsKey(cliid))
                        {
                            Clients[cliid].Sensors.Add(senid, new SensorEx(Sensors[senid], con.SensorIndex));
                        }
                    }
                    foreach (String cliid in Clients.Keys)
                    {
                        Client cli = Clients[cliid];
                        String insid = cli.Institute.ToString("N");
                        if (Institutes.ContainsKey(insid))
                        {
                            Institutes[insid].Clients.Add(cliid, cli);
                        }
                    }
                }
                #endregion

                #region Report
                using (ReportDbContext db_rpt = new ReportDbContext())
                {
                    foreach (ReportInfo rpt in db_rpt.Reports)
                    {
                        if (rpt.Enabled)
                        {
                            Institutes[rpt.Institute.ToString("N")].Reports.Add(rpt.Id.ToString("N"), rpt.Name);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when loading db context.", ex);
                HttpRuntime.UnloadAppDomain();
                return false;
            }
            return true;
        }
    }
}