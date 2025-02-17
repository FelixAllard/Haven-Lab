# Deployment
## Welcome to the deployment guide
# Requirement
Hardware :
- RAM : 6 GO
- CPU THREADS : 3
- STORAGE SPACE : 5 GO
- INTERNET : 30 MBPS

To check, use
[▶ checks_specs.bat](./Windows/check_specs.bat)

Software
- NPM
- .NET 8. 0+
- EF Migration

Use [▶ check_environment.bat](./Windows/check_environment.bat) to check if everything is installed

Then use [▶ install_ef_migration.bat](./Windows/install_ef_migration.bat). This will be necessary in order to prepare the database.


# Deployment with one machine
In this section, we will be looking at how to deploy the system on a single machine. First lets start with the basics. 
The minimum requirements for this system is 6 GB or ram, 3 threads and approximately 5gb of free storage to handle additional cache.

You can run the [▶ checks_specs.bat](./Windows/check_specs.bat) to check your computers specs

Here is how the system looks like

![C3.png](Images/C3.png)

As you can see, the application connects to external services, and in order to do so, it will need some API keys.
The system will work even if you don't provide those, but be very careful because it will be connected to the temporary development keys and as such will not result in desired output.
To start the deployment process, run the

## MAKE SURE YOUR SOLUTION IS ON TEH LATEST VERSION

Run the [▶ import_and_reset.bat](../import_and_reset.bat)
THIS WILL DELETE ANY LOCAL CHANGES AND IMPORT THE REMOTE MAIN ON THE CURRENT ONE

## Step 1
Run the [▶ checks_specs.bat](./Windows/check_specs.bat)

Make sure your specs follow the following requirements : 
- RAM : 6 GO
- CPU THREADS : 3
- STORAGE SPACE : 5 GO



Now that this is done, we are ready for deployment
## Step 2
Run the [▶ generate_env.bat](./Windows/generate_env.bat)

This should generate a file at [▶ .env](../.env)

Here is an overview of what the .env should look like

```dotenv
#THIS .ENV must be placed in the root repository of the project, else it will not be considered
#This is the base information and the software should work with those informations
#I suggest switching the JWT_KEY and connect to the REAL Shopify_API

#DATABASE CONNECTION
GLOBAL_DB_HOST="localhost"
GLOBAL_DB_PORT="3310"
GLOBAL_DB_USER="user"
GLOBAL_DB_PASSWORD="pwd"
#Api Gateway
ENV_BASE_URL_APPOINTMENT_API="http://localhost:5114"
ENV_BASE_URL_APIWEBAUTH_API="http://localhost:5113"
ENV_BASE_URL_SHOPIFY_API="http://localhost:5106"
ENV_BASE_URL_EMAIL_API="http://localhost:5092"

#Api Web Auth
APIWEBAUTH_JWT_KEY="f978bc16f0f8de?770aa5f5adf8d-0eede2bce000a"

#Appointment Service
#---
#Email Api
EMAIL_API_HOST="localhost"
EMAIL_API_PORT=1025
EMAIL_API_ENABLE_SSL=false
EMAIL_API_USER=""
EMAIL_API_PASSWORD=""

#Shopify API
SHOPIFY_API_SHOP_URL="vc-shopz.myshopify.com"
SHOPIFY_API_ACCESS_TOKEN="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"
```
---

Let's break this down
```dotenv
GLOBAL_DB_HOST="localhost"
GLOBAL_DB_PORT="3310"
GLOBAL_DB_USER="user"
GLOBAL_DB_PASSWORD="pwd"
```
This is to connect with the database. Be very careful because you will have to make sure it can connect to the database.

To check if the connection is possible, you can run the [▶ check_db_connection.bat](./Windows/check_db_connection.bat)

This will tell you if it is possible to connect to the database with the information inside the .env file

---

Now for the API gateway part of the .env, those address must be connected to the address the other services are hosted on. 

Just put the __**address and the port**__ plus if it is HTTP or HTTPS, nothing else!
__If you are running the API Gateway and the other services on the same machine, excluding the front end, you won't need to change those address, but if you put the api gateway on a remote machine, it is imperative that you change those address.
```dotenv
ENV_BASE_URL_APPOINTMENT_API="http://localhost:5114"
ENV_BASE_URL_APIWEBAUTH_API="http://localhost:5113"
ENV_BASE_URL_SHOPIFY_API="http://localhost:5106"
ENV_BASE_URL_EMAIL_API="http://localhost:5092"
```

---

The API WEB AUTH part of the .env is very simple,
```dotenv
APIWEBAUTH_JWT_KEY="f978bc16f0f8de?770aa5f5adf8d-0eede2bce000a"
```

A default JWT key is provided, but I would suggest switching it to another key so that people can't look at the git and know the key.

---

### Email Api ENV Variables
```dotenv
EMAIL_API_HOST="localhost"
EMAIL_API_PORT=1025
EMAIL_API_ENABLE_SSL=false
EMAIL_API_USER=""
EMAIL_API_PASSWORD=""
```
This is the connection to the SMTP service you will be using. You can use a local one like the one provided through the docker, or you can use an online existing one such as GOOGLE.

---

### Shopify API
```dotenv
SHOPIFY_API_SHOP_URL="vc-shopz.myshopify.com"
SHOPIFY_API_ACCESS_TOKEN="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"
```
This is the most important thing to change. Those values are the one we used throughout development and this store will not be used in the final build.
Make sure to swap the SHOP URL with the actual URL to your shop.
You will then need to create a ACCESS TOKEN for your shop. Here is how you do it.

To learn how to create a SHOPIFY ACCESS TOKEN, go to [Create Access Token](./ShopifySetup.md#accesstoken)

---

Now that our ENV is Setup, we are ready to start launching our application.

## Step 3
Applying migrations.
Make sure the connection credentials you provided actually are for an existing database.
If you are not sure if you have the migration application tool, run the following to install it.

[▶ install_ef_migration.bat](./Windows/install_ef_migration.bat)


Now that you have the tool installed, build your project

To build your project, you can run the [▶ build_solution.bat](./Windows/build_solution.bat) or do it manually.

Now that the project is built, you can run the migrations with the script :  [▶ apply_migrations.bat](./Windows/apply_migrations.bat).

Once again this process can be done manually. If the migrations are successful. You can proceed to the last step of deployment.

Now one last important step before we can actually get done with this, we will need to tell the front end where to search for the backend.

Go to [▶ Deployment Env Front end](../front-end/.env.production) and type the IP the backend is on.

## Step 4

You can now launch the launch.bat that is the [▶ launch.bat](../launch.bat)

This will take care of launching the solution, it will also rebuild. The backend takes 8 seconds to go up if already built if you have 8 threads and 30 seconds if not built yet.




