namespace NetExam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetExam.Abstractions;
    using NetExam.Dto;

    public class OfficeRental : IOfficeRental
    {
        private List<LocationSpecs> Locations = new List<LocationSpecs>();

        private List<OfficeSpecs> Office = new List<OfficeSpecs>();

        private List<BookingRequest> Bookings = new List<BookingRequest>();

        public void AddLocation(LocationSpecs locationSpecs)
        {
            if(this.CheckIfLocationExists(locationSpecs.Name))
            {
                throw new Exception($"Location {locationSpecs.Name} already exist");
            }
            this.Locations.Add(locationSpecs);
        }

        public void AddOffice(OfficeSpecs officeSpecs)
        {
            if (!this.CheckIfLocationExists(officeSpecs.LocationName))
            {
                throw new Exception($"Selected locatio {officeSpecs.LocationName} doesn't exists");
            }
            this.Office.Add(officeSpecs);
        }

        public void BookOffice(BookingRequest bookingRequest)
        {
            if (this.Bookings.Find((booking)=> booking.OfficeName == bookingRequest.OfficeName) != null)
            {
                throw new Exception($"Selected booking with office {bookingRequest.OfficeName} already exists");
            }
            this.Bookings.Add(bookingRequest);
        }

        public IEnumerable<IBooking> GetBookings(string locationName, string officeName)
        {
            return this.Bookings;
        }

        public IEnumerable<ILocation> GetLocations()
        {
            return this.Locations;
        }

        public IEnumerable<IOffice> GetOffices(string locationName)
        {
            return this.Office.FindAll((office)=> office.LocationName == locationName);
        }

        public IEnumerable<IOffice> GetOfficeSuggestion(SuggestionRequest suggestionRequest)
        {
            IEnumerable<IOffice> capacityNeeded = this.Office.FindAll((office) => office.MaxCapacity >= suggestionRequest.CapacityNeeded);

            IEnumerable<IOffice> preferedNeigboorHood = this.Office.FindAll((office) => this.CheckPreferentNeigborHood(office.LocationName, suggestionRequest.PreferedNeigborHood));
            
            IEnumerable<IOffice> resoursesNeeded = this.Office.FindAll((office) => this.CheckResourses(office.AvailableResources, suggestionRequest.ResourcesNeeded));
          
            return capacityNeeded.Intersect(preferedNeigboorHood.Union(capacityNeeded)).Intersect(resoursesNeeded).OrderByDescending((office)=> office.Name);
        }

        private bool CheckResourses(IEnumerable<string> availables, IEnumerable<string> resoursesNeeded)
        {
            IEnumerable<string> result = availables.Where((available) => resoursesNeeded.Contains(available));
            return result.Count() == resoursesNeeded.Count();
        }

        private bool CheckPreferentNeigborHood(string locationName, string preferNeigborHood)
        {
            LocationSpecs location = this.Locations.Find((l) => l.Name == locationName && l.Neighborhood == preferNeigborHood);
            return location != null || preferNeigborHood == null;

        }

        private bool CheckIfLocationExists(string locationName)
        {
            return this.Locations.Find((location) => location.Name == locationName) != null;
        }
    }
}