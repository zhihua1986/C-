using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Customers
{
    public class ViewModel : INotifyPropertyChanged
    {
        private List<Customer> customers;
        private int currentCustomer;
        public Command NextCustomer { get; private set; }
        public Command PreviousCustomer { get; private set; }
        public Command FirstCustomer { get; private set; }
        public Command LastCustomer { get; private set; }
        public Command AddCustomer { get; private set; }
        public Command EditCustomer { get; private set; }
        public Command DiscardChanges { get; private set; }
        public Command SaveChanges { get; private set; }

        private const string ServerUrl = "http://<webappname>.azurewebsites.net/";
        private HttpClient client = null;

        public ViewModel()
        {
            this.currentCustomer = 0;
            this.IsAtStart = true;
            this.IsAtEnd = false;
            this.NextCustomer = new Command(this.Next, 
                () => { return this.CanBrowse && 
                        this.customers != null && !this.IsAtEnd; });
            this.PreviousCustomer = new Command(this.Previous, 
                () => { return this.CanBrowse && 
                        this.customers != null && !this.IsAtStart; });
            this.FirstCustomer = new Command(this.First, 
                () => { return this.CanBrowse && 
                        this.customers != null && !this.IsAtStart; });
            this.LastCustomer = new Command(this.Last,
                () => { return this.CanBrowse && 
                        this.customers != null && !this.IsAtEnd; });
            this.AddCustomer = new Command(this.Add,
                () => { return this.CanBrowse; });
            this.EditCustomer = new Command(this.Edit,
                () => { return this.CanBrowse; });
            this.DiscardChanges = new Command(this.Discard,
                () => { return this.CanSaveOrDiscardChanges; });
            this.SaveChanges = new Command(this.SaveAsync, 
                () => { return this.CanSaveOrDiscardChanges; });


            this.customers = null;
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(ServerUrl);
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Methods for fetching and updating data

        // Create a new (empty) customer 
        // and put the form into Adding mode
        private void Add()
        {
            Customer newCustomer = new Customer { CustomerID = 0 };
            this.customers.Insert(currentCustomer, newCustomer);
            this.IsAdding = true;
            this.OnPropertyChanged(nameof(Current));
        }

        // Edit the current customer 
        // - save the existing details of the customer
        //   and put the form into Editing mode
        private Customer oldCustomer;

        private void Edit()
        {
            this.oldCustomer = new Customer();
            this.CopyCustomer(this.Current, this.oldCustomer);
            this.IsEditing = true;
        }

        // Discard changes made while in Adding or Editing mode
        // and return the form to Browsing mode
        private void Discard()
        {
            // If the user was adding a new customer, then remove it
            if (this.IsAdding)
            {
                this.customers.Remove(this.Current);
                this.OnPropertyChanged(nameof(Current));
            }

            // If the user was editing an existing customer, 
            // then restore the saved details
            if (this.IsEditing)
            {
                this.CopyCustomer(this.oldCustomer, this.Current);
            }

            this.IsBrowsing = true;
            this.LastError = String.Empty;
        }

        // Save the new or updated customer back to the web service
        // and return the form to Browsing mode
        private async void SaveAsync()
        {
            // Validate the details of the Customer
            if (this.ValidateCustomer(this.Current))
            {
                // Only continue if the customer details are valid
                this.IsBusy = true;
                try
                {
                    // Convert the current customer into HTTP request format with a JSON payload
                    var serializedData = JsonConvert.SerializeObject(this.Current);
                    StringContent content =
                        new StringContent(serializedData, Encoding.UTF8, "text/json");

                    // If the user is adding a new customer,
                    // send an HTTP POST request to the web service with the details
                    if (this.IsAdding)
                    {
                        var response =
                            await client.PostAsync("api/customers", content);
                        if (response.IsSuccessStatusCode)
                        {
                            // Get the ID of the newly created customer and display it
                            Uri customerUri = response.Headers.Location;
                            var newCust = await this.client.GetAsync(customerUri);
                            if (newCust.IsSuccessStatusCode)
                            {
                                var customerData = await newCust.Content.ReadAsStringAsync();
                                this.CopyCustomer(
                                    JsonConvert.DeserializeObject<Customer>(customerData), this.Current);
                                this.OnPropertyChanged(nameof(Current));
                                this.IsAdding = false;
                                this.IsBrowsing = true;
                                this.LastError = String.Empty;
                            }
                            else
                            {
                                // TODO: Handle GET failure
                                this.LastError = response.ReasonPhrase;
                            }
                        }
                        // TODO: Handle POST failure
                        else
                        {
                            this.LastError = response.ReasonPhrase;
                        }
                    }
                    // The user must be editing an existing customer,
                    // so send the details by using a PUT request 
                    else
                    {
                        string path = $"api/customers/{this.Current.CustomerID}";

                        var response = await client.PutAsync(path, content);
                        if (response.IsSuccessStatusCode)
                        {
                            this.IsEditing = false;
                            this.IsBrowsing = true;
                            this.LastError = String.Empty;
                        }
                        // TODO: Handle PUT failure
                        else
                        {
                            this.LastError = response.ReasonPhrase;
                        }
                    }
                }
                catch (Exception e)
                {
                    // TODO: Handle exceptions
                    this.LastError = e.Message;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }

        // Helper method to validate customer details
        private bool ValidateCustomer(Customer customer)
        {
            string validationErrors = string.Empty;
            bool hasErrors = false;

            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                hasErrors = true;
                validationErrors = "First Name must not be empty\n";
            }

            if (string.IsNullOrWhiteSpace(customer.LastName))
            {
                hasErrors = true;
                validationErrors += "Last Name must not be empty\n";
            }

            // Email address is a series of characters that do not include a space or @
            // followed by @
            // followed by a series of characters that do not include a space or @
            // followed by .
            // followed by a series of characters that do not include a space or @
            Regex emailRegex = new Regex(@"^[^@ ]+@[^@ ]+\.[^@ ]+$");
            if (string.IsNullOrWhiteSpace(customer.EmailAddress) ||
                !emailRegex.IsMatch(customer.EmailAddress))
            {
                hasErrors = true;
                validationErrors += "Invalid Email Address\n";
            }

            // Phone number is a series of digits, brackets, spaces, +, and - characters
            Regex phoneRegex = new Regex(@"^[0-9\(\)\/+ \-]+$");
            if (string.IsNullOrWhiteSpace(customer.Phone) ||
                !phoneRegex.IsMatch(customer.Phone))
            {
                hasErrors = true;
                validationErrors += "Invalid Phone Number\n";
            }

            this.LastError = validationErrors;
            return !hasErrors;
        }

        // Utility method for copying the details of a customer
        private void CopyCustomer(Customer source, Customer destination)
        {
            destination.CustomerID = source.CustomerID;
            destination.EmailAddress = source.EmailAddress;
            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.Phone = source.Phone;
            destination.Title = source.Title;
            destination.rowguid = source.rowguid;
            destination.ModifiedDate = source.ModifiedDate;
        }

        public async Task GetDataAsync()
        {
            try
            {
                this.IsBusy = true;
                //await Task.Delay(5000);
                var response = await this.client.GetAsync("api/customers");
                if (response.IsSuccessStatusCode)
                {
                    var customerData =
                        await response.Content.ReadAsStringAsync();

                    this.customers = JsonConvert.DeserializeObject<List<Customer>>(customerData);

                    this.currentCustomer = 0;
                    this.OnPropertyChanged(nameof(Current));
                    this.IsAtStart = true;
                    this.IsAtEnd = (this.customers.Count == 0);
                    this.LastError = String.Empty;
                }
                else
                {
                    this.LastError = response.ReasonPhrase;
                }
            }
            catch (Exception e)
            {
                this.LastError = e.Message;
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        #endregion

        #region Properties for "busy" and error message handling

        private bool _isBusy;
        public bool IsBusy
        {
            get { return this._isBusy; }
            set 
            {
                this._isBusy = value;
                this.OnPropertyChanged(nameof(IsBusy));
            }
        }

        private string _lastError = null;
        public string LastError
        {
            get { return this._lastError; }
            private set
            {
                this._lastError = value;
                this.OnPropertyChanged(nameof(LastError));
            }
        }

        #endregion

        #region Properties for managing the edit mode

        // Manage the edit mode of the form - is the user browsing, adding a customer, or editing a customer
        private enum EditMode { Browsing, Adding, Editing };
        private EditMode editMode;
        public bool IsBrowsing
        {
            get { return this.editMode == EditMode.Browsing; }
            private set
            {
                if (value)
                {
                    this.editMode = EditMode.Browsing;
                }
                this.OnPropertyChanged(nameof(IsBrowsing));
                this.OnPropertyChanged(nameof(IsAdding));
                this.OnPropertyChanged(nameof(IsEditing));
                this.OnPropertyChanged(nameof(IsAddingOrEditing));
            }
        }

        public bool IsAdding
        {
            get { return this.editMode == EditMode.Adding; }
            private set
            {
                if (value)
                {
                    this.editMode = EditMode.Adding;
                }
                this.OnPropertyChanged(nameof(IsBrowsing));
                this.OnPropertyChanged(nameof(IsAdding));
                this.OnPropertyChanged(nameof(IsEditing));
                this.OnPropertyChanged(nameof(IsAddingOrEditing));
            }
        }

        public bool IsEditing
        {
            get { return this.editMode == EditMode.Editing; }
            private set
            {
                if (value)
                {
                    this.editMode = EditMode.Editing;
                }
                this.OnPropertyChanged(nameof(IsBrowsing));
                this.OnPropertyChanged(nameof(IsAdding));
                this.OnPropertyChanged(nameof(IsEditing));
                this.OnPropertyChanged(nameof(IsAddingOrEditing));
            }
        }

        public bool IsAddingOrEditing
        {
            get { return this.IsAdding || this.IsEditing; }
        }

        private bool CanBrowse
        {
            get
            {
                return this.IsBrowsing &&
                   this.client != null;
            }
        }

        private bool CanSaveOrDiscardChanges
        {
            get
            {
                return this.IsAddingOrEditing &&
                       this.client != null;
            }
        }

        #endregion

        #region Methods and properties for navigation commands

        private bool _isAtStart;
        public bool IsAtStart
        {
            get { return this._isAtStart; }
            set
            {
                this._isAtStart = value;
                this.OnPropertyChanged(nameof(IsAtStart));
            }
        }

        private bool _isAtEnd;
        public bool IsAtEnd
        {
            get { return this._isAtEnd; }
            set
            {
                this._isAtEnd = value;
                this.OnPropertyChanged(nameof(IsAtEnd));
            }
        }

        public Customer Current
        {
            get
            {
                if (this.customers != null)
                {
                    return this.customers[currentCustomer];
                }
                else
                {
                    return null;
                }
            }
        }

        private void Next()
        {
            if (this.customers.Count - 1 > this.currentCustomer)
            {
                this.currentCustomer++;
                this.OnPropertyChanged(nameof(Current));
                this.IsAtStart = false;
                this.IsAtEnd = (this.customers.Count - 1 == this.currentCustomer);
            }
        }

        private void Previous()
        {
            if (this.currentCustomer > 0)
            {
                this.currentCustomer--;
                this.OnPropertyChanged(nameof(Current));
                this.IsAtEnd = false;
                this.IsAtStart = (this.currentCustomer == 0);
            }
        }

        private void First()
        {
            this.currentCustomer = 0;
            this.OnPropertyChanged(nameof(Current));
            this.IsAtStart = true;
            this.IsAtEnd = (this.customers.Count == 0);
        }

        private void Last()
        {
            this.currentCustomer = this.customers.Count - 1;
            this.OnPropertyChanged(nameof(Current));
            this.IsAtEnd = true;
            this.IsAtStart = (this.customers.Count == 0);
        }

        #endregion

        #region INotifyPropertyChanged interface

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
