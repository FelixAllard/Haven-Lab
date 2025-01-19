# Use the official Node.js image to build the React app
FROM node:18-alpine AS build

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json
COPY package.json package-lock.json ./ 

# Install dependencies
RUN npm install

# Copy the rest of the application files
COPY . . 

# Build the React application
RUN npm run build

# Use the official Nginx image as the base image
FROM nginx:alpine

# Copy the custom nginx.conf file into the container
COPY ./nginx.conf /etc/nginx/nginx.conf

# Copy the build folder of the React app into Nginx's default folder
COPY ./build /usr/share/nginx/html

# Expose port 80 for the web server
EXPOSE 80

# Start Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]

