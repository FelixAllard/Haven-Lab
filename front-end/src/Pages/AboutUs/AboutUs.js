import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import { motion } from 'motion/react';

// Temporary stock image URL (You can replace this with real image URLs later)
const placeholderImage = 'https://via.placeholder.com/150';

// Data for team members
const teamMembers = [
  {
    name: 'John Doe',
    role: 'Co-Founder',
    description:
      'John has been instrumental in the vision and direction of the company.',
    fullDescription:
      'John’s vision has shaped the company from the ground up. His leadership continues to guide the team to achieve our goals, while maintaining the company’s core values of innovation and teamwork.',
  },
  {
    name: 'Jane Smith',
    role: 'Co-Founder',
    description:
      'Jane leads the strategy and operations with a focus on innovation and growth.',
    fullDescription:
      'With an eye for strategy, Jane has developed the blueprint for our growth and ensures our operations are aligned with the industry’s best practices. She is passionate about pushing the boundaries of what’s possible.',
  },
  {
    name: 'Emma Johnson',
    role: 'Creative Director',
    description:
      'Emma brings creativity and design expertise to shape our brand identity.',
    fullDescription:
      'Emma’s designs have redefined our brand’s visual identity, ensuring that it resonates with our audience on a deeper level. She continuously explores new trends and adapts them to our work.',
  },
];

const AboutUs = () => {
  const [selectedMember, setSelectedMember] = useState(null);
  const [isExpanded, setIsExpanded] = useState(false);

  const handleLearnMoreClick = (member) => {
    setSelectedMember(member);
    setIsExpanded(true);
  };

  const handleCloseModal = () => {
    setIsExpanded(false);
    setSelectedMember(null);
  };

  return (
    <div className="mt-6">
      <Container className="py-5 bg-dark text-light">
        <Row className="mb-4">
          <Col>
            <motion.h1
              className="text-center"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1 }}
            >
              About Us
            </motion.h1>
            <motion.p
              className="text-center"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1.5 }}
            >
              We are a passionate team dedicated to making a difference in the
              world.
            </motion.p>
          </Col>
        </Row>

        <Row>
          {teamMembers.map((member, index) => (
            <Col sm={12} md={4} key={index} className="mb-4">
              <motion.div
                initial={{ opacity: 0, scale: 0.8 }}
                animate={{ opacity: 1, scale: 1 }}
                transition={{ delay: 0.2 * index, duration: 0.8 }}
              >
                <Card className="bg-dark text-light">
                  <Card.Img variant="top" src={placeholderImage} />
                  <Card.Body>
                    <Card.Title>{member.name}</Card.Title>
                    <Card.Subtitle className="mb-2 text-muted">
                      {member.role}
                    </Card.Subtitle>
                    <Card.Text>{member.description}</Card.Text>
                    <motion.div
                      initial={{ scale: 1 }}
                      animate={{ scale: isExpanded ? 0 : 1 }}
                      transition={{ duration: 0.5 }}
                    >
                      <Button
                        variant="light"
                        onClick={() => handleLearnMoreClick(member)}
                      >
                        Learn More
                      </Button>
                    </motion.div>
                  </Card.Body>
                </Card>
              </motion.div>
            </Col>
          ))}
        </Row>

        {/* Modal-style Popup for "Learn More" */}
        {isExpanded && selectedMember && (
          <motion.div
            className="position-fixed top-0 left-0 w-100 h-100 bg-dark bg-opacity-75 d-flex justify-content-center align-items-center"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.5 }}
          >
            <motion.div
              className="bg-light text-dark p-4 rounded"
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{ duration: 0.5 }}
            >
              <h2>{selectedMember.name}</h2>
              <h4>{selectedMember.role}</h4>
              <p>{selectedMember.fullDescription}</p>
              <Button variant="dark" onClick={handleCloseModal}>
                Close
              </Button>
            </motion.div>
          </motion.div>
        )}
      </Container>
    </div>
  );
};

export default AboutUs;
