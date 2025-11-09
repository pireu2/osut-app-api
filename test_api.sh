#!/bin/bash

# OSUT App API Test Script
# Tests all endpoints of the OSUT application
# Admin User GUID: b6f4b7f2-bba3-44b2-b387-1f031ce0a1fa

set -e

# Configuration
BASE_URL="http://localhost:5285"
ADMIN_USER_ID="b6f4b7f2-bba3-44b2-b387-1f031ce0a1fa"
GOOGLE_ID_TOKEN=""
# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Global variables
ACCESS_TOKEN=""
REFRESH_TOKEN=""

# Helper functions
print_header() {
    echo -e "\n${BLUE}========================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

make_request() {
    local method=$1
    local url=$2
    local data=$3
    local auth_header=""

    if [ -n "$ACCESS_TOKEN" ]; then
        auth_header="-H \"Authorization: Bearer $ACCESS_TOKEN\""
    fi

    local curl_cmd="curl -s -X $method \"$BASE_URL$url\""

    if [ -n "$data" ]; then
        # Use a temporary file to avoid JSON formatting issues
        local temp_file=$(mktemp)
        echo "$data" > "$temp_file"
        curl_cmd="$curl_cmd -H \"Content-Type: application/json\" -d @$temp_file"
        # Clean up temp file after command
        trap "rm -f $temp_file" RETURN
    fi

    if [ -n "$auth_header" ]; then
        curl_cmd="$curl_cmd $auth_header"
    fi

    eval "$curl_cmd"
}

# Authentication functions
setup_authentication() {
    print_header "Setting up Authentication"

    # Test login with real Google ID token
    echo "Testing login with Google ID token..."
    local login_response=$(make_request POST "/api/auth/login" "{\"idToken\":\"$GOOGLE_ID_TOKEN\"}")

    if echo "$login_response" | grep -q "accessToken"; then
        print_success "Login successful with Google ID token"
        # Extract tokens from response using more robust parsing
        ACCESS_TOKEN=$(echo "$login_response" | sed -n 's/.*"accessToken"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')
        REFRESH_TOKEN=$(echo "$login_response" | sed -n 's/.*"refreshToken"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')

        if [ -n "$ACCESS_TOKEN" ] && [ -n "$REFRESH_TOKEN" ]; then
            print_success "Access token and refresh token extracted successfully"
            echo "Access Token: ${ACCESS_TOKEN:0:50}..."
            echo "Refresh Token: ${REFRESH_TOKEN:0:50}..."
        else
            print_error "Failed to extract tokens from login response"
            print_error "Response: $login_response"
            exit 1
        fi
    else
        print_error "Login failed with Google ID token"
        print_error "Response: $login_response"
        print_warning "Make sure the Google ID token is valid and the user is whitelisted"
        exit 1
    fi
}

# Test functions for each controller
test_auth_endpoints() {
    print_header "Testing Authentication Endpoints"

    # Login test (already done in setup_authentication)
    if [ -n "$ACCESS_TOKEN" ] && [ -n "$REFRESH_TOKEN" ]; then
        print_success "Login endpoint works (tested during setup)"
    else
        print_error "Login endpoint failed during setup"
        return 1
    fi

    # Test refresh token
    echo "Testing refresh token endpoint..."
    local refresh_response=$(make_request POST "/api/auth/refresh" "{\"refreshToken\":\"$REFRESH_TOKEN\"}")
    if echo "$refresh_response" | grep -q "accessToken"; then
        print_success "Refresh token endpoint works"
        local new_access_token=$(echo "$refresh_response" | sed -n 's/.*"accessToken"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')
        local new_refresh_token=$(echo "$refresh_response" | sed -n 's/.*"refreshToken"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')

        if [ -n "$new_access_token" ] && [ -n "$new_refresh_token" ]; then
            ACCESS_TOKEN="$new_access_token"
            REFRESH_TOKEN="$new_refresh_token"
            print_success "Tokens refreshed successfully"
        fi
    else
        print_error "Refresh token endpoint failed: $refresh_response"
    fi

    # Test logout
    echo "Testing logout endpoint..."
    local logout_response=$(make_request POST "/api/auth/logout" "{\"refreshToken\":\"$REFRESH_TOKEN\"}")
    if [ $? -eq 0 ]; then
        print_success "Logout endpoint works"
        print_warning "Note: User is now logged out. Re-authenticating for remaining tests..."
        # Re-authenticate for remaining tests
        setup_authentication
    else
        print_error "Logout endpoint failed: $logout_response"
    fi
}

test_user_endpoints() {
    print_header "Testing User Endpoints"

    # Get all users
    echo "Testing GET /api/users..."
    local users_response=$(make_request GET "/api/users")
    if [ $? -eq 0 ]; then
        print_success "Get all users endpoint works"
        echo "Response: $users_response"
    else
        print_error "Get all users endpoint failed"
    fi

    # Get specific user (admin user)
    echo "Testing GET /api/users/$ADMIN_USER_ID..."
    local user_response=$(make_request GET "/api/users/$ADMIN_USER_ID")
    if [ $? -eq 0 ]; then
        print_success "Get user by ID endpoint works"
        echo "Response: $user_response"
    else
        print_error "Get user by ID endpoint failed"
    fi

    # Update user
    echo "Testing PUT /api/users/$ADMIN_USER_ID..."
    local update_data="{
        \"id\": \"$ADMIN_USER_ID\",
        \"firstName\": \"Admin\",
        \"lastName\": \"User\",
        \"yearOfBirth\": 1990,
        \"status\": \"ActiveMember\",
        \"isAdmin\": true,
        \"email\": \"admin@example.com\",
        \"userName\": \"admin\"
    }"
    local update_response=$(make_request PUT "/api/users/$ADMIN_USER_ID" "$update_data")
    if [ $? -eq 0 ]; then
        print_success "Update user endpoint works"
        echo "Response: $update_response"
    else
        print_error "Update user endpoint failed"
    fi
}

test_department_endpoints() {
    print_header "Testing Department Endpoints"

    # Get all departments
    echo "Testing GET /api/departments..."
    local depts_response=$(make_request GET "/api/departments")
    if [ $? -eq 0 ]; then
        print_success "Get all departments endpoint works"
        echo "Response: $depts_response"
    else
        print_error "Get all departments endpoint failed"
    fi

    # Create a test department
    echo "Testing POST /api/departments..."
    local dept_data="{
        \"name\": \"Test Department\",
        \"description\": \"A test department for API testing\",
        \"type\": \"Projects\",
        \"coordinatorId\": \"$ADMIN_USER_ID\"
    }"
    local create_dept_response=$(make_request POST "/api/departments" "$dept_data")
    if [ $? -eq 0 ]; then
        print_success "Create department endpoint works"
        echo "Response: $create_dept_response"

        # Extract department ID for further testing
        local dept_id=$(echo "$create_dept_response" | grep -o '"id":"[^"]*' | cut -d'"' -f4 | head -1 | tr -d '\n\r\t ')
        if [ -n "$dept_id" ]; then
            echo "Created department ID: $dept_id"

            # Test get department by ID
            echo "Testing GET /api/departments/$dept_id..."
            local get_dept_response=$(make_request GET "/api/departments/$dept_id")
            if [ $? -eq 0 ]; then
                print_success "Get department by ID endpoint works"
            else
                print_error "Get department by ID endpoint failed"
            fi

            # Test update department
            echo "Testing PUT /api/departments/$dept_id..."
            local update_dept_data="{
                \"id\": \"$dept_id\",
                \"name\": \"Updated Test Department\",
                \"description\": \"Updated description\",
                \"type\": \"Services\",
                \"coordinatorId\": \"$ADMIN_USER_ID\"
            }"
            local update_dept_response=$(make_request PUT "/api/departments/$dept_id" "$update_dept_data")
            if [ $? -eq 0 ]; then
                print_success "Update department endpoint works"
            else
                print_error "Update department endpoint failed"
            fi

            # Test delete department
            echo "Testing DELETE /api/departments/$dept_id..."
            local delete_dept_response=$(make_request DELETE "/api/departments/$dept_id")
            if [ $? -eq 0 ]; then
                print_success "Delete department endpoint works"
            else
                print_error "Delete department endpoint failed"
            fi
        fi
    else
        print_error "Create department endpoint failed: $create_dept_response"
    fi

    # Test get departments by type
    echo "Testing GET /api/departments/type/Projects..."
    local depts_by_type_response=$(make_request GET "/api/departments/type/Projects")
    if [ $? -eq 0 ]; then
        print_success "Get departments by type endpoint works"
    else
        print_error "Get departments by type endpoint failed"
    fi
}

test_board_member_endpoints() {
    print_header "Testing Board Member Endpoints"

    # Clean up any existing board members for this user to avoid conflicts
    echo "Cleaning up existing board members for test user..."
    local existing_board_members=$(make_request GET "/api/boardmembers/user/$ADMIN_USER_ID")
    local existing_id=$(echo "$existing_board_members" | grep -o '"id":"[^"]*' | cut -d'"' -f4 | head -1)
    if [ -n "$existing_id" ]; then
        make_request DELETE "/api/boardmembers/$existing_id" > /dev/null 2>&1
        echo "Removed existing board member: $existing_id"
    fi

    # Get all board members
    echo "Testing GET /api/boardmembers..."
    local board_members_response=$(make_request GET "/api/boardmembers")
    if [ $? -eq 0 ]; then
        print_success "Get all board members endpoint works"
        echo "Response: $board_members_response"
    else
        print_error "Get all board members endpoint failed"
    fi

    # Create a test board member
    echo "Testing POST /api/boardmembers..."
    local board_member_data="{
        \"userId\": \"$ADMIN_USER_ID\",
        \"position\": \"VicePresidentElectronics\",
        \"assignedDate\": \"$(date -I)\"
    }"
    local create_board_member_response=$(make_request POST "/api/boardmembers" "$board_member_data")
    if [ $? -eq 0 ]; then
        print_success "Create board member endpoint works"
        echo "Response: $create_board_member_response"

        # Extract board member ID for further testing
        local board_member_id=$(echo "$create_board_member_response" | grep -o '"id":"[^"]*' | cut -d'"' -f4 | head -1 | tr -d '\n\r\t ')
        if [ -n "$board_member_id" ]; then
            echo "Created board member ID: $board_member_id"

            # Test get board member by ID
            echo "Testing GET /api/boardmembers/$board_member_id..."
            local get_board_member_response=$(make_request GET "/api/boardmembers/$board_member_id")
            if [ $? -eq 0 ]; then
                print_success "Get board member by ID endpoint works"
            else
                print_error "Get board member by ID endpoint failed"
            fi

            # Test update board member
            echo "Testing PUT /api/boardmembers/$board_member_id..."
            local update_board_member_data="{
                \"id\": \"$board_member_id\",
                \"userId\": \"$ADMIN_USER_ID\",
                \"position\": \"VicePresidentConstruction\",
                \"assignedDate\": \"$(date -I)\"
            }"
            local update_board_member_response=$(make_request PUT "/api/boardmembers/$board_member_id" "$update_board_member_data")
            if [ $? -eq 0 ]; then
                print_success "Update board member endpoint works"
            else
                print_error "Update board member endpoint failed"
            fi

            # Test delete board member
            echo "Testing DELETE /api/boardmembers/$board_member_id..."
            local delete_board_member_response=$(make_request DELETE "/api/boardmembers/$board_member_id")
            if [ $? -eq 0 ]; then
                print_success "Delete board member endpoint works"
            else
                print_error "Delete board member endpoint failed"
            fi
        fi
    else
        print_error "Create board member endpoint failed: $create_board_member_response"
    fi

    # Test get board member by user ID
    echo "Testing GET /api/boardmembers/user/$ADMIN_USER_ID..."
    local board_member_by_user_response=$(make_request GET "/api/boardmembers/user/$ADMIN_USER_ID")
    if [ $? -eq 0 ]; then
        print_success "Get board member by user ID endpoint works"
    else
        print_error "Get board member by user ID endpoint failed"
    fi
}

test_event_endpoints() {
    print_header "Testing Event Endpoints"

    # Get all events
    echo "Testing GET /api/events..."
    local events_response=$(make_request GET "/api/events")
    if [ $? -eq 0 ]; then
        print_success "Get all events endpoint works"
        echo "Response: $events_response"
    else
        print_error "Get all events endpoint failed"
    fi

    # Get upcoming events
    echo "Testing GET /api/events/upcoming..."
    local upcoming_events_response=$(make_request GET "/api/events/upcoming")
    if [ $? -eq 0 ]; then
        print_success "Get upcoming events endpoint works"
    else
        print_error "Get upcoming events endpoint failed"
    fi

    # First, we need a department to create an event
    # Let's create a temporary department for testing
    local temp_dept_data="{
        \"name\": \"Event Test Department\",
        \"description\": \"Temporary department for event testing\",
        \"type\": \"Projects\",
        \"coordinatorId\": \"$ADMIN_USER_ID\"
    }"
    local temp_dept_response=$(make_request POST "/api/departments" "$temp_dept_data")
    # Extract and clean department ID - be very aggressive about removing whitespace
    local temp_dept_id=$(echo "$temp_dept_response" | grep -o '"id":"[^"]*"' | head -1 | sed 's/"id":"//;s/"//;s/[[:space:]]//g')

    if [ -n "$temp_dept_id" ]; then
        # Clean up any whitespace/newlines from the ID
        temp_dept_id=$(echo "$temp_dept_id" | tr -d '\n\r\t ')
        echo "Created temporary department for event testing: $temp_dept_id"

        # Create a test event
        echo "Testing POST /api/events..."
        local future_date="2025-12-01T14:00:00Z"
        
        # Try with minimal required fields first
        local event_data="{\"title\":\"Test Event\",\"dateTime\":\"$future_date\",\"location\":\"Test Location\",\"departmentId\":\"$temp_dept_id\"}"
        
        # Debug: Show the exact JSON being sent
        echo "Sending JSON: $event_data"
        echo "Department ID: '$temp_dept_id'"
        
        local create_event_response=$(make_request POST "/api/events" "$event_data")
        if [ $? -eq 0 ]; then
            print_success "Create event endpoint works"
            echo "Response: $create_event_response"

            # Extract event ID for further testing
            local event_id=$(echo "$create_event_response" | grep -o '"id":"[^"]*' | cut -d'"' -f4 | head -1 | tr -d '\n\r\t ')
            if [ -n "$event_id" ]; then
                echo "Created event ID: $event_id"

                # Test get event by ID
                echo "Testing GET /api/events/$event_id..."
                local get_event_response=$(make_request GET "/api/events/$event_id")
                if [ $? -eq 0 ]; then
                    print_success "Get event by ID endpoint works"
                else
                    print_error "Get event by ID endpoint failed"
                fi

                # Test update event
                echo "Testing PUT /api/events/$event_id..."
                local update_event_data="{
                    \"id\": \"$event_id\",
                    \"title\": \"Updated Test Event\",
                    \"description\": \"Updated description\",
                    \"dateTime\": \"$future_date\",
                    \"location\": \"Updated Location\",
                    \"departmentId\": \"$temp_dept_id\"
                }"
                local update_event_response=$(make_request PUT "/api/events/$event_id" "$update_event_data")
                if [ $? -eq 0 ]; then
                    print_success "Update event endpoint works"
                else
                    print_error "Update event endpoint failed"
                fi

                # Test signup for event
                echo "Testing POST /api/events/$event_id/signup..."
                local signup_response=$(make_request POST "/api/events/$event_id/signup")
                if [ $? -eq 0 ]; then
                    print_success "Signup for event endpoint works"
                else
                    print_error "Signup for event endpoint failed"
                fi

                # Test get event signups
                echo "Testing GET /api/events/$event_id/signups..."
                local signups_response=$(make_request GET "/api/events/$event_id/signups")
                if [ $? -eq 0 ]; then
                    print_success "Get event signups endpoint works"
                else
                    print_error "Get event signups endpoint failed"
                fi

                # Test cancel signup
                echo "Testing DELETE /api/events/$event_id/signup..."
                local cancel_signup_response=$(make_request DELETE "/api/events/$event_id/signup")
                if [ $? -eq 0 ]; then
                    print_success "Cancel signup endpoint works"
                else
                    print_error "Cancel signup endpoint failed"
                fi

                # Test delete event
                echo "Testing DELETE /api/events/$event_id..."
                local delete_event_response=$(make_request DELETE "/api/events/$event_id")
                if [ $? -eq 0 ]; then
                    print_success "Delete event endpoint works"
                else
                    print_error "Delete event endpoint failed"
                fi
            fi
        else
            print_error "Create event endpoint failed: $create_event_response"
        fi

        # Clean up temporary department
        echo "Cleaning up temporary department..."
        make_request DELETE "/api/departments/$temp_dept_id" > /dev/null 2>&1
    else
        print_error "Could not create temporary department for event testing: $temp_dept_response"
        print_warning "Skipping event endpoint tests due to department creation failure"
    fi
}

# Cleanup functions
cleanup_test_departments() {
    print_header "Cleaning up leftover test departments"

    local depts_response=$(make_request GET "/api/departments")

    if command -v jq >/dev/null 2>&1; then
        # Use jq to parse JSON and find test departments
        local test_dept_ids=$(echo "$depts_response" | jq -r '.[] | select(.name | contains("Test Department") or contains("Event Test Department")) | .id')

        for id in $test_dept_ids; do
            if [ -n "$id" ] && [ "$id" != "null" ]; then
                echo "Deleting leftover test department: $id"
                make_request DELETE "/api/departments/$id" >/dev/null 2>&1
                if [ $? -eq 0 ]; then
                    print_success "Deleted test department $id"
                else
                    print_warning "Failed to delete test department $id"
                fi
            fi
        done
    else
        print_warning "jq not available, skipping cleanup of leftover test departments. Install jq for better cleanup."
    fi
}

# Main test execution
main() {
    echo -e "${BLUE}OSUT App API Test Suite${NC}"
    echo "Base URL: $BASE_URL"
    echo "Admin User ID: $ADMIN_USER_ID"
    echo "Started at: $(date)"

    # Setup authentication with real Google ID token
    setup_authentication

    # Clean up any leftover test data from previous runs
    cleanup_test_departments

    # Run all endpoint tests
    test_auth_endpoints
    test_user_endpoints
    test_department_endpoints
    test_board_member_endpoints
    test_event_endpoints

    # Final cleanup of any test data created during this run
    cleanup_test_departments

    print_header "Test Suite Complete"
    echo "Finished at: $(date)"
    echo -e "${GREEN}All tests completed!${NC}"
}

# Check if the API is running
check_api_status() {
    echo "Checking if API is running at $BASE_URL..."
    # Test with a simple endpoint that doesn't require auth
    if curl -s --max-time 5 "$BASE_URL/api/auth/login" > /dev/null 2>&1; then
        print_success "API is running and accessible"
        return 0
    else
        print_error "API is not running or not accessible at $BASE_URL"
        print_warning "Please start the API server first:"
        echo "  cd OsutApp.Api"
        echo "  dotnet run"
        exit 1
    fi
}

# Run pre-flight checks
check_api_status

# Run the main test suite
main
