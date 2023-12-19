FROM node:20.10.0 as base

WORKDIR /app
COPY . .

RUN npm install --verbose

EXPOSE 4200

CMD ["npm", "run", "start:container"]
