import { GraphQLClient, gql } from 'graphql-request';
import { createClient } from 'graphql-ws';
import WebSocket from 'ws';

const GATEWAY_URL = process.env.GATEWAY_URL || 'http://localhost:5000/graphql';
const GATEWAY_WS_URL = process.env.GATEWAY_WS_URL || 'ws://localhost:5001/graphql';

// Create GraphQL client for queries and mutations
const client = new GraphQLClient(GATEWAY_URL);

// Example 1: Query products through gateway
async function queryProducts() {
  console.log('\n=== Querying Products ===');
  const query = gql`
    query {
      products(first: 5) {
        nodes {
          id
          name
          description
          price
          category {
            name
          }
        }
      }
    }
  `;

  try {
    const data = await client.request(query);
    console.log('Products:', JSON.stringify(data, null, 2));
  } catch (error) {
    console.error('Error querying products:', error.message);
  }
}

// Example 2: Query product with reviews
async function queryProductWithReviews() {
  console.log('\n=== Querying Product with Reviews ===');
  const query = gql`
    query {
      products(where: { id: { eq: 1 } }) {
        nodes {
          id
          name
          reviews(first: 5) {
            nodes {
              author
              comment
              rating
            }
            totalCount
          }
        }
      }
    }
  `;

  try {
    const data = await client.request(query);
    console.log('Product with Reviews:', JSON.stringify(data, null, 2));
  } catch (error) {
    console.error('Error querying product with reviews:', error.message);
  }
}

// Example 3: Add a review via gateway mutation
async function addReview() {
  console.log('\n=== Adding Review ===');
  const mutation = gql`
    mutation {
      addReview(input: { 
        productId: 1, 
        author: "GraphQL Client", 
        comment: "Testing mutation through gateway", 
        rating: 5 
      }) {
        review {
          id
          author
          comment
          rating
        }
      }
    }
  `;

  try {
    const data = await client.request(mutation);
    console.log('Added Review:', JSON.stringify(data, null, 2));
  } catch (error) {
    console.error('Error adding review:', error.message);
  }
}

// Example 4: Subscribe to product additions (connects directly to Products service)
function subscribeToProductAdded() {
  console.log('\n=== Subscribing to Product Added ===');
  console.log('Listening for new products (press Ctrl+C to stop)...');

  const wsClient = createClient({
    url: GATEWAY_WS_URL,
    webSocketImpl: WebSocket,
  });

  const subscription = gql`
    subscription {
      productAdded {
        id
        name
        description
        price
      }
    }
  `;

  const unsubscribe = wsClient.subscribe(
    {
      query: subscription,
    },
    {
      next: (data) => {
        console.log('New Product Added:', JSON.stringify(data, null, 2));
      },
      error: (error) => {
        console.error('Subscription error:', error);
      },
      complete: () => {
        console.log('Subscription complete');
      },
    }
  );

  // Keep the subscription alive
  return unsubscribe;
}

// Run examples
async function main() {
  console.log('GraphQL Client Demo');
  console.log('===================');

  // Run query examples
  await queryProducts();
  await queryProductWithReviews();
  await addReview();

  // Run subscription (comment this out if you don't want to keep the process running)
  // const unsubscribe = subscribeToProductAdded();
  
  console.log('\n=== Demo Complete ===');
  console.log('To test subscriptions, uncomment the subscription code in client.js');
  console.log('Then run: node client.js');
  console.log('In another terminal, add a product via mutation to see the subscription trigger.');
}

main().catch(console.error);
