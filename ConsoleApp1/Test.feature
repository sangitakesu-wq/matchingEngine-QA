Feature: MatchingEngine 
  In order to verify the Distribution Processing solution content
  As a user of the MatchingEngine site
  I want to open the Solutions menu, navigate to Distribution Processing and verify the "All-in-one solution for scale" section

@MatchingEngine @Solutions @Chrome
  Scenario: Verify Distribution Processing 'All-in-one solution for scale' content
	Given I open the MatchingEngine homepage
	And I accept cookies
	When I expand the Solutions menu in the header
	Then I should see a list of Solutions displayed
	When I click "Distribution Processing" from the Solutions list
	And I scroll to the "All-in-one solution for scale" section
	Then I should see content present in the "All-in-one solution for scale" section

